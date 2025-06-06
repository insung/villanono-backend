using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Options;
using OpenSearch.Client;

public sealed class VillanonoElasticSearchRepository : IVillanonoRepository
{
    readonly OpenSearchClient opensearchClient;
    readonly string defaultIndex;
    private const string locationsIndex = "villanono-locations";

    public VillanonoElasticSearchRepository(
        OpenSearchClient elasticsearchClient,
        IOptions<ElasticSearchSettingsModel> elasticSearchSettings
    )
    {
        this.opensearchClient = elasticsearchClient;
        defaultIndex = elasticSearchSettings.Value.DefaultIndex;
    }

    public async ValueTask Ping()
    {
        var response = await opensearchClient.PingAsync();
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Ping failed"
        );
    }

    #region IndexManagement
    public async ValueTask<string> CreateIndex(string indexName)
    {
        var response = await opensearchClient.Indices.CreateAsync(indexName);
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Create Index failed"
        );
        return indexName;
    }

    public async ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel
    {
        var response = await opensearchClient.Indices.CreateAsync(
            indexName,
            c => c.Map<T>(m => m.Properties(p => p.Text(t => t.Name(n => n.AddressNumber))))
        );
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Create DataIndex failed"
        );
    }

    public async ValueTask CreateLocationsIndex()
    {
        var response = await opensearchClient.Indices.CreateAsync(locationsIndex);
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Create Index failed"
        );
    }

    public async ValueTask<bool> HasIndex(string indexName)
    {
        var response = await opensearchClient.Indices.ExistsAsync(indexName);
        return response.Exists;
    }

    public async ValueTask DeleteIndex(string indexName)
    {
        var response = await opensearchClient.Indices.DeleteAsync(indexName);
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "DeleteIndex failed"
        );
    }

    public async ValueTask ReIndex(string sourceIndexName, string targetIndexName)
    {
        var response = await opensearchClient.ReindexOnServerAsync(
            new ReindexOnServerRequest
            {
                Source = new ReindexSource { Index = sourceIndexName },
                Destination = new ReindexDestination { Index = targetIndexName },
            }
        );
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "ReIndex failed"
        );
    }
    #endregion

    #region Private Methods
    private bool tryGetElasticSearchApiResponseCode(
        int? httpStatusCode,
        string? debugInformation,
        out HttpStatusCode responseCode,
        out Exception? innerException
    )
    {
        innerException = null;
        var apiCallResponseCode = httpStatusCode ?? 500;
        responseCode = (HttpStatusCode)apiCallResponseCode;
        if (responseCode != HttpStatusCode.OK)
        {
            innerException = new Exception(debugInformation ?? "");
        }
        return responseCode == HttpStatusCode.OK;
    }

    private void CheckResponseFailed(
        int? httpStatusCode,
        string? debugInformation,
        string failedMessage
    )
    {
        if (
            !tryGetElasticSearchApiResponseCode(
                httpStatusCode,
                debugInformation,
                out HttpStatusCode responseCode,
                out Exception? innerException
            )
        )
        {
            throw new HttpRequestException(failedMessage, innerException, responseCode);
        }
    }
    #endregion

    #region Insert/BulkInsert
    public async Task BulkInsertData<T>(List<T> records, string? indexName = null)
        where T : VillanonoBaseModel
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            indexName = defaultIndex;
        }

        var response = await opensearchClient.BulkAsync(b => b.Index(indexName).IndexMany(records));
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "BulkInsert failed"
        );
    }

    public async Task BulkInsertLocations<T>(List<T> records)
        where T : VillanonoBaseModel
    {
        if (!await HasIndex(locationsIndex))
            await CreateLocationsIndex();

        var bulkOperations = new ConcurrentBag<IBulkOperation>();

        Parallel.ForEach(
            records,
            record =>
            {
                var id = $"{record.Si}-{record.Gu}-{record.Dong}";
                var locationRecord = new LocationModel
                {
                    Si = record.Si,
                    Gu = record.Gu,
                    Dong = record.Dong,
                };
                var bulkOperaion = new BulkCreateOperation<object>(locationRecord) { Id = id };
                bulkOperations.Add(bulkOperaion);
            }
        );

        var bulkRequest = new BulkRequest(locationsIndex) { Operations = bulkOperations.ToList() };
        var response = await opensearchClient.BulkAsync(bulkRequest);
    }
    #endregion

    #region Read Operations
    public async Task<IReadOnlyCollection<T>> GetData<T>(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel
    {
        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery { Field = Infer.Field<T>(f => f.DataType), Value = dataType },
                    new MatchQuery { Field = Infer.Field<T>(f => f.Dong), Query = dong },
                    new MatchQuery { Field = Infer.Field<T>(f => f.Gu), Query = gu },
                    new MatchQuery { Field = Infer.Field<T>(f => f.Si), Query = si },
                },
                Filter = new QueryContainer[]
                {
                    new DateRangeQuery
                    {
                        Field = Infer.Field<T>(f => f.ContractDate),
                        GreaterThanOrEqualTo = beginDate.ToString("yyyyMMdd"),
                        LessThanOrEqualTo = endDate.ToString("yyyyMMdd"),
                    },
                },
            },
        };

        var response = await opensearchClient.SearchAsync<T>(searchRequest);
        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetData Failed"
        );
        return response?.Documents ?? Array.Empty<T>();
    }

    public async Task<InsightReportDailyModel> GetReportInsightDaily(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    )
    {
        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery { Field = "dataType", Value = dataType },
                    new MatchQuery { Field = "dong", Query = dong },
                    new MatchQuery { Field = "gu", Query = gu },
                    new MatchQuery { Field = "si", Query = si },
                },
                Filter = new QueryContainer[]
                {
                    new DateRangeQuery
                    {
                        Field = "contractDate",
                        GreaterThanOrEqualTo = beginDate.ToString("yyyyMMdd"),
                        LessThanOrEqualTo = endDate.ToString("yyyyMMdd"),
                    },
                },
            },
            Aggregations = new AggregationDictionary
            {
                {
                    "contractDateGroup",
                    new HistogramAggregation("contractDateGroup")
                    {
                        Field = "contractDate",
                        Interval = 1,
                        Aggregations = new AggregationDictionary
                        {
                            { "stats", new StatsAggregation("stats", "transactionAmount") },
                            {
                                "percentiles",
                                new PercentilesAggregation("percentiles", "transactionAmount")
                                {
                                    Percents = new[] { 25.0, 50.0, 75.0 },
                                }
                            },
                        },
                    }
                },
                { "totalStats", new StatsAggregation("totalStats", "transactionAmount") },
                {
                    "totalPercentiles",
                    new PercentilesAggregation("totalPercentiles", "transactionAmount")
                    {
                        Percents = new[] { 25.0, 50.0, 75.0 },
                    }
                },
            },
            Size = 0,
        };

        var response = await opensearchClient.SearchAsync<object>(searchRequest);

        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetStatisticsSummary Failed"
        );

        if (response == null)
            throw new InvalidOperationException("OpenSearch Response cannot be null.");

        var totalStats = response.Aggregations.ExtendedStats("totalStats");
        var totalPercentiles = response.Aggregations.Percentiles("totalPercentiles");
        var contractDateGroup = response.Aggregations.Histogram("contractDateGroup");
        return new InsightReportDailyModel(
            beginDate,
            endDate,
            totalStats,
            totalPercentiles,
            contractDateGroup
        );
    }

    public async Task<InsightReportMonthlyModel> GetReportInsightMonthly(
        VillanonoDataType dataType,
        int beginYearMonth,
        int endYearMonth,
        string dong,
        string gu,
        string si = "서울특별시",
        double? exclusiveAreaBegin = null,
        double? exclusiveAreaEnd = null,
        int? constructionYear = null,
        string indexName = "villanono-*"
    )
    {
        var filters = new List<QueryContainer>
        {
            new NumericRangeQuery
            {
                Field = "contractYearMonth",
                GreaterThanOrEqualTo = beginYearMonth,
                LessThanOrEqualTo = endYearMonth,
            },
        };

        if (exclusiveAreaBegin != null && exclusiveAreaEnd != null)
        {
            filters.Add(
                new NumericRangeQuery
                {
                    Field = "exclusiveArea",
                    GreaterThanOrEqualTo = exclusiveAreaBegin,
                    LessThanOrEqualTo = exclusiveAreaEnd,
                }
            );
        }

        if (constructionYear != null)
        {
            filters.Add(
                new NumericRangeQuery
                {
                    Field = "constructionYear",
                    GreaterThanOrEqualTo = constructionYear,
                }
            );
        }

        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery { Field = "dataType", Value = dataType },
                    new MatchQuery { Field = "dong", Query = dong },
                    new MatchQuery { Field = "gu", Query = gu },
                    new MatchQuery { Field = "si", Query = si },
                },
                Filter = filters,
            },
            Aggregations = new AggregationDictionary
            {
                {
                    "contractDateGroup",
                    new HistogramAggregation("contractDateGroup")
                    {
                        Field = "contractYearMonth",
                        Interval = 1,
                        Aggregations = new AggregationDictionary
                        {
                            {
                                "extendedStats",
                                new ExtendedStatsAggregation("extendedStats", "transactionAmount")
                            },
                            {
                                "percentiles",
                                new PercentilesAggregation("percentiles", "transactionAmount")
                                {
                                    Percents = new[] { 25.0, 50.0, 75.0 },
                                }
                            },
                        },
                    }
                },
                { "totalStats", new ExtendedStatsAggregation("totalStats", "transactionAmount") },
                {
                    "totalPercentiles",
                    new PercentilesAggregation("totalPercentiles", "transactionAmount")
                    {
                        Percents = new[] { 25.0, 50.0, 75.0 },
                    }
                },
            },
            Size = 0,
        };

        var response = await opensearchClient.SearchAsync<object>(searchRequest);

        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetStatisticsSummary Failed"
        );

        if (response == null)
            throw new InvalidOperationException("OpenSearch Response cannot be null.");

        var totalStats = response.Aggregations.ExtendedStats("totalStats");
        var totalPercentiles = response.Aggregations.Percentiles("totalPercentiles");
        var contractDateGroup = response.Aggregations.Histogram("contractDateGroup");

        return new InsightReportMonthlyModel(
            beginYearMonth,
            endYearMonth,
            totalStats,
            totalPercentiles,
            contractDateGroup
        );
    }

    public async Task<IList<string>> GetAllSi()
    {
        var response = await opensearchClient.SearchAsync<LocationModel>(s =>
            s.Index(locationsIndex)
                .Size(0) // 결과 문서는 필요하지 않으므로 Size 0
                .Aggregations(a =>
                    a.Terms(
                        "si",
                        t =>
                            t.Field(f => f.Si.Suffix("keyword")) // 분석되지 않은 필드 사용
                                .Size(100) // 충분한 버킷 수
                    )
                )
        );

        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetAllSi Failed"
        );

        var result = response
            .Aggregations.Terms("si")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<string>> GetAllGu(string Si)
    {
        var response = await opensearchClient.SearchAsync<LocationModel>(s =>
            s.Index(locationsIndex)
                .Size(0) // 결과 문서는 필요하지 않으므로 Size 0
                .Query(q => q.Term(t => t.Field(f => f.Si.Suffix("keyword")).Value(Si)))
                .Aggregations(a =>
                    a.Terms("gu", t => t.Field(f => f.Gu.Suffix("keyword")).Size(100))
                )
        );

        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetAllSi Failed"
        );

        var result = response
            .Aggregations.Terms("gu")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<string>> GetAllDong(string Si, string Gu)
    {
        var response = await opensearchClient.SearchAsync<LocationModel>(s =>
            s.Index(locationsIndex)
                .Size(0) // 결과 문서는 필요하지 않으므로 Size 0
                .Query(q =>
                    q.Bool(b =>
                        b.Must(
                            mq => mq.Term(t => t.Field(f => f.Si.Suffix("keyword")).Value(Si)),
                            mq => mq.Term(t => t.Field(f => f.Gu.Suffix("keyword")).Value(Gu))
                        )
                    )
                )
                .Aggregations(a =>
                    a.Terms("dong", t => t.Field(f => f.Dong.Suffix("keyword")).Size(100))
                )
        );

        CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetAllSi Failed"
        );

        var result = response
            .Aggregations.Terms("dong")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }
    #endregion
}

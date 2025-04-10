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
            "Create Index failed"
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
                var locationRecord = new
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

    public async Task<IReadOnlyCollection<T>> GetData<T>(
        VillanonoDataType dataType,
        int beginDate,
        int endDate,
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
                        GreaterThanOrEqualTo = beginDate.ToString(),
                        LessThanOrEqualTo = endDate.ToString(),
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

    public async Task<StatisticalSummary> GetStatisticsSummary(
        VillanonoDataType dataType,
        int beginDate,
        int endDate,
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
                        GreaterThanOrEqualTo = beginDate.ToString(),
                        LessThanOrEqualTo = endDate.ToString(),
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

        var totalStats = response.Aggregations.Stats("totalStats");
        var totalPercentiles = response.Aggregations.Percentiles("totalPercentiles");
        var contractDateGroup = response.Aggregations.Histogram("contractDateGroup");
        return new StatisticalSummary(
            beginDate,
            endDate,
            totalStats,
            totalPercentiles,
            contractDateGroup
        );
    }
}

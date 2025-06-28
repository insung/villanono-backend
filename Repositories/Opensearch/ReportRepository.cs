using OpenSearch.Client;

public class ReportRepository : IReportRepository
{
    readonly OpenSearchClient opensearchClient;

    public ReportRepository(OpenSearchClient elasticsearchClient)
    {
        this.opensearchClient = elasticsearchClient;
    }

    public async Task<InsightReportDailyModel> GetInsightDaily(
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

        OpensearchResponseHandler.CheckResponseFailed(
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

    public async Task<InsightReportMonthlyModel> GetInsightMonthly(
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

        OpensearchResponseHandler.CheckResponseFailed(
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
}

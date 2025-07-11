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
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null,
        int? beginConstructYear = null,
        int? endConstructYear = null,
        string indexName = "villanono-*"
    )
    {
        var filters = new List<QueryContainer>
        {
            new TermQuery { Field = "dataType", Value = (int)dataType },
            new TermQuery { Field = "si.keyword", Value = si },
            new DateRangeQuery
            {
                Field = "contractDate",
                GreaterThanOrEqualTo = beginDate.ToString("yyyyMMdd"),
                LessThanOrEqualTo = endDate.ToString("yyyyMMdd"),
            },
        };

        if (!string.IsNullOrWhiteSpace(gu))
        {
            filters.Add(new TermQuery { Field = "gu.keyword", Value = gu });
        }

        if (!string.IsNullOrWhiteSpace(dong))
        {
            filters.Add(new TermQuery { Field = "dong.keyword", Value = dong });
        }

        if (beginExclusiveArea.HasValue || endExclusiveArea.HasValue)
        {
            var rangeQuery = new NumericRangeQuery { Field = "exclusiveArea" };
            if (beginExclusiveArea.HasValue)
                rangeQuery.GreaterThanOrEqualTo = beginExclusiveArea.Value;
            if (endExclusiveArea.HasValue)
                rangeQuery.LessThanOrEqualTo = endExclusiveArea.Value;
            filters.Add(rangeQuery);
        }

        if (beginConstructYear.HasValue || endConstructYear.HasValue)
        {
            var rangeQuery = new NumericRangeQuery { Field = "constructionYear" };
            if (beginConstructYear.HasValue)
                rangeQuery.GreaterThanOrEqualTo = beginConstructYear.Value;
            if (endConstructYear.HasValue)
                rangeQuery.LessThanOrEqualTo = endConstructYear.Value;
            filters.Add(rangeQuery);
        }

        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery { Filter = filters },
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
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null,
        int? beginConstructYear = null,
        int? endConstructYear = null,
        string indexName = "villanono-*"
    )
    {
        var filters = new List<QueryContainer>
        {
            new TermQuery { Field = "dataType", Value = (int)dataType },
            new TermQuery { Field = "si.keyword", Value = si },
            new NumericRangeQuery
            {
                Field = "contractYearMonth",
                GreaterThanOrEqualTo = beginYearMonth,
                LessThanOrEqualTo = endYearMonth,
            },
        };

        if (!string.IsNullOrWhiteSpace(gu))
        {
            filters.Add(new TermQuery { Field = "gu.keyword", Value = gu });
        }

        if (!string.IsNullOrWhiteSpace(dong))
        {
            filters.Add(new TermQuery { Field = "dong.keyword", Value = dong });
        }

        if (beginExclusiveArea.HasValue || endExclusiveArea.HasValue)
        {
            var rangeQuery = new NumericRangeQuery { Field = "exclusiveArea" };
            if (beginExclusiveArea.HasValue)
                rangeQuery.GreaterThanOrEqualTo = beginExclusiveArea.Value;
            if (endExclusiveArea.HasValue)
                rangeQuery.LessThanOrEqualTo = endExclusiveArea.Value;
            filters.Add(rangeQuery);
        }

        if (beginConstructYear.HasValue || endConstructYear.HasValue)
        {
            var rangeQuery = new NumericRangeQuery { Field = "constructionYear" };
            if (beginConstructYear.HasValue)
                rangeQuery.GreaterThanOrEqualTo = beginConstructYear.Value;
            if (endConstructYear.HasValue)
                rangeQuery.LessThanOrEqualTo = endConstructYear.Value;
            filters.Add(rangeQuery);
        }

        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery { Filter = filters },
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

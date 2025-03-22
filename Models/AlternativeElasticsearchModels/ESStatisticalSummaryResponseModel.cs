using System.Text.Json.Serialization;

public class ESResponse
{
    [JsonPropertyName("took")]
    public long Took { get; set; }

    [JsonPropertyName("timed_out")]
    public bool TimedOut { get; set; }

    [JsonPropertyName("_shards")]
    public ESResponseShards Shards { get; set; }

    [JsonPropertyName("hits")]
    public ESHits Hits { get; set; }

    public class ESResponseShards
    {
        [JsonPropertyName("total")]
        public long Total { get; set; }

        [JsonPropertyName("successful")]
        public long Successful { get; set; }

        [JsonPropertyName("skipped")]
        public long Skipped { get; set; }

        [JsonPropertyName("failed")]
        public long Failed { get; set; }
    }

    public class ESHits
    {
        [JsonPropertyName("total")]
        public ESHitsTotal Total { get; set; }

        [JsonPropertyName("max_score")]
        public string? MaxScore { get; set; }

        [JsonPropertyName("hits")]
        public List<object> Hits { get; set; }

        public class ESHitsTotal
        {
            [JsonPropertyName("value")]
            public long Value { get; set; }

            [JsonPropertyName("relation")]
            public string Relation { get; set; }
        }
    }
}

public class ESAggregations<T> : ESResponse
{
    [JsonPropertyName("aggregations")]
    public T Aggregations { get; set; }
}

public class ESResponseAggregation<T>
{
    [JsonPropertyName("doc_count_error_upper_bound")]
    public long DocCountErrorUpperBound { get; set; }

    [JsonPropertyName("sum_other_doc_count")]
    public long SumOtherDocCount { get; set; }

    [JsonPropertyName("buckets")]
    public List<T> Buckets { get; set; }
}

public class ESValues<T>
{
    [JsonPropertyName("values")]
    public Dictionary<string, T> Values { get; set; }
}

public class ESValue<T>
{
    [JsonPropertyName("value")]
    public T Value { get; set; }
}

public class ESStatisticsSummaryResponse
{
    [JsonPropertyName("key")]
    public ESResponseAggregation<ESStatisticsSummary> Key { get; set; }

    public long TotalCount
    {
        get { return Key.Buckets.Select(x => x.Count).Sum(); }
    }

    public double TotalAverage
    {
        get { return Key.Buckets.Select(x => x.Average).Average(); }
    }

    public double StdDev
    {
        get
        {
            double varianceSum = 0.0;
            foreach (var value in Key.Buckets.Select(x => x.Average))
            {
                varianceSum += Math.Pow(value - TotalAverage, 2);
            }
            double variance = varianceSum / Key.Buckets.Count;
            return Math.Sqrt(variance);
        }
    }
}

public class ESStatisticsSummary
{
    [JsonPropertyName("key")]
    public int Key { get; set; }

    [JsonPropertyName("doc_count")]
    public long Count { get; set; }

    [JsonInclude]
    [JsonPropertyName("average")]
    private ESValue<double> _average { get; set; }

    public double Average
    {
        get { return _average.Value; }
    }

    [JsonPropertyName("min")]
    [JsonInclude]
    private ESValue<double> _min { get; set; }

    public double Min
    {
        get { return _min.Value; }
    }

    [JsonPropertyName("max")]
    [JsonInclude]
    private ESValue<double> _max { get; set; }

    public double Max
    {
        get { return _max.Value; }
    }

    [JsonPropertyName("percentiles")]
    [JsonInclude]
    private ESValues<double> _percentiles { get; set; }

    public double Percentiles25
    {
        get { return _percentiles.Values["25.0"]; }
    }
    public double Percentiles50
    {
        get { return _percentiles.Values["50.0"]; }
    }
    public double Percentiles75
    {
        get { return _percentiles.Values["75.0"]; }
    }
}




// public class StatisticalSummary
// {
//     public long CountOfRows { get; set; } = 0;
//     public double AverageOfTransactionAmount { get; set; } = 0.0;
//     public IEnumerable<StatisticsBase>? Statistics { get; set; }

//     public class StatisticsBase
//     {
//         public long Key { get; set; }
//         public long Count { get; set; }
//         public double Min { get; set; }
//         public double Max { get; set; }
//         public double Average { get; set; }
//         public double StdDev { get; set; }
//         public double Percentiles25 { get; set; }
//         public double Percentiles50 { get; set; }
//         public double Percentiles75 { get; set; }

//         public StatisticsBase(
//             long key,
//             long count,
//             double min,
//             double max,
//             double average,
//             double stdDev,
//             List<double> percentiles
//         )
//         {
//             Key = key;
//             Count = count;
//             Min = min;
//             Max = max;
//             Average = average;
//             StdDev = stdDev;
//             (Percentiles25, Percentiles50, Percentiles75) = GetPercentiles(percentiles);
//         }

//         private static (
//             double Percentile25,
//             double Percentile50,
//             double Percentile75
//         ) GetPercentiles(List<double> sortedValues)
//         {
//             double Percentile(double p)
//             {
//                 var index = p * (sortedValues.Count - 1);
//                 var lower = (int)Math.Floor(index);
//                 var upper = (int)Math.Ceiling(index);
//                 return sortedValues[lower]
//                     + (index - lower) * (sortedValues[upper] - sortedValues[lower]);
//             }

//             return (
//                 Percentile25: Percentile(0.25),
//                 Percentile50: Percentile(0.50), // Median
//                 Percentile75: Percentile(0.75)
//             );
//         }

//         private double GetPercentile(List<double> sortedValues, double percentile)
//         {
//             var index = percentile * (sortedValues.Count - 1);
//             var lower = (int)Math.Floor(index);
//             var upper = (int)Math.Ceiling(index);
//             return sortedValues[lower]
//                 + (index - lower) * (sortedValues[upper] - sortedValues[lower]);
//         }
//     }

//     public static StatisticalSummary Convert(IReadOnlyCollection<VillanonoBaseModel> models)
//     {
//         var statistics = models
//             .GroupBy(x => x.ContractYearMonth)
//             .Select(g => new StatisticsBase(
//                 key: g.Key,
//                 count: g.Count(),
//                 min: g.Min(x => x.TransactionAmount),
//                 max: g.Max(x => x.TransactionAmount),
//                 average: g.Average(x => x.TransactionAmount),
//                 stdDev: Math.Sqrt(
//                     g.Average(x =>
//                         Math.Pow(x.TransactionAmount - g.Average(x2 => x2.TransactionAmount), 2)
//                     )
//                 ),
//                 percentiles: g.Select(x => x.TransactionAmount).OrderBy(s => s).ToList()
//             ));
//         return new StatisticalSummary
//         {
//             CountOfRows = models.Count,
//             AverageOfTransactionAmount = models.Average(x => x.TransactionAmount),
//             Statistics = statistics,
//         };
//     }
// }

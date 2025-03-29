using OpenSearch.Client;

public class StatisticalSummary
{
    public long TotalCount { get; set; }
    public double? TotalAverage { get; set; }
    public double StdDev { get; set; }
    public List<StatisticsSummaryItem> Items { get; set; }

    public StatisticalSummary(
        int beginDate,
        int endDate,
        StatsAggregate? totalStats,
        MultiBucketAggregate<KeyedBucket<double>>? contractDateBuckets
    )
    {
        if (totalStats == null)
            throw new ArgumentNullException(
                "Failed to create StatisticalSummary: totalStats cannot be null."
            );

        if (contractDateBuckets == null)
            throw new ArgumentNullException(
                "Failed to create StatisticalSummary: contractDateBuckets cannot be null."
            );

        TotalCount = totalStats.Count;
        TotalAverage = totalStats.Average;

        Items = new List<StatisticsSummaryItem>();

        for (int i = beginDate; i <= endDate; i++)
        {
            var whereCondition = contractDateBuckets.Buckets.Where(b => b.Key == i);
            if (whereCondition.Any())
            {
                var groupBy = whereCondition.First();
                var stats = groupBy.Stats("stats");
                var percentiles = groupBy.Percentiles("percentiles");

                Items.Add(
                    new StatisticsSummaryItem
                    {
                        ContractDate = (int)groupBy.Key,
                        Count = stats.Count,
                        Average = stats.Average,
                        Min = stats.Min,
                        Max = stats.Max,
                        Sum = stats.Sum,
                        Percentiles25 = percentiles.Items.First(p => p.Percentile == 25.0).Value,
                        Percentiles50 = percentiles.Items.First(p => p.Percentile == 50.0).Value,
                        Percentiles75 = percentiles.Items.First(p => p.Percentile == 75.0).Value,
                    }
                );
            }
            else
            {
                Items.Add(new StatisticsSummaryItem { ContractDate = i });
            }
        }

        double varianceSum = 0.0;
        foreach (var average in Items.Select(x => x.Average))
        {
            varianceSum += Math.Pow(average ?? 0 - TotalAverage ?? 0, 2);
        }
        double variance = varianceSum / TotalCount;
        StdDev = Math.Sqrt(variance);
    }
}

public class StatisticsSummaryItem
{
    public int ContractDate { get; set; }
    public long Count { get; set; }
    public double? Average { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double Sum { get; set; }
    public double? Percentiles25 { get; set; }
    public double? Percentiles50 { get; set; }
    public double? Percentiles75 { get; set; }
}

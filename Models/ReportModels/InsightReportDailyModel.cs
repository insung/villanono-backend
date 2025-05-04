using OpenSearch.Client;

public class InsightReportDailyModel : InsightReportBaseModel
{
    public InsightReportDailyModel(
        DateOnly beginDate,
        DateOnly endDate,
        ExtendedStatsAggregate? totalStats,
        PercentilesAggregate? totalPercentiles,
        MultiBucketAggregate<KeyedBucket<double>>? contractDateBuckets
    )
    {
        if (totalStats == null)
            throw new ArgumentNullException(
                "Failed to create StatisticalSummary: totalStats cannot be null."
            );

        if (totalPercentiles == null)
            throw new ArgumentNullException(
                "Failed to create StatisticalSummary: totalPercentiles cannot be null."
            );

        if (contractDateBuckets == null)
            throw new ArgumentNullException(
                "Failed to create StatisticalSummary: contractDateBuckets cannot be null."
            );

        Items = new List<InsightSummaryItem>();

        for (DateOnly date = beginDate; date <= endDate; date = date.AddDays(1))
        {
            var intDate = ConvertInt32(date);
            var whereCondition = contractDateBuckets.Buckets.Where(b => b.Key == intDate);
            if (whereCondition.Any())
            {
                var groupBy = whereCondition.First();
                var stats = groupBy.ExtendedStats("extendedStats");
                var percentiles = groupBy.Percentiles("percentiles");

                Items.Add(
                    new InsightSummaryItem
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
                        StdDev = stats.StdDeviation,
                    }
                );
            }
            else
            {
                Items.Add(new InsightSummaryItem { ContractDate = intDate });
            }
        }

        Total = new InsightSummary
        {
            Count = totalStats.Count,
            Average = totalStats.Average,
            Min = totalStats.Min,
            Max = totalStats.Max,
            Sum = totalStats.Sum,
            Percentiles25 = totalPercentiles.Items.First(p => p.Percentile == 25.0).Value,
            Percentiles50 = totalPercentiles.Items.First(p => p.Percentile == 50.0).Value,
            Percentiles75 = totalPercentiles.Items.First(p => p.Percentile == 75.0).Value,
            StdDev = totalStats.StdDeviation,
        };
    }
}

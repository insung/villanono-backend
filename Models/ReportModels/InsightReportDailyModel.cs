using OpenSearch.Client;

public class InsightReportDailyModel : InsightReportBaseModel
{
    public InsightReportDailyModel(
        DateOnly beginDate,
        DateOnly endDate,
        StatsAggregate? totalStats,
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
                var stats = groupBy.Stats("stats");
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
                    }
                );
            }
            else
            {
                Items.Add(new InsightSummaryItem { ContractDate = intDate });
            }
        }

        Total = new InsightSummary(
            count: totalStats.Count,
            average: totalStats.Average,
            min: totalStats.Min,
            max: totalStats.Max,
            sum: totalStats.Sum,
            percentiles25: totalPercentiles.Items.First(p => p.Percentile == 25.0).Value,
            percentiles50: totalPercentiles.Items.First(p => p.Percentile == 50.0).Value,
            percentiles75: totalPercentiles.Items.First(p => p.Percentile == 75.0).Value,
            itemsAverage: Items.Select(x => x.Average)
        );
    }
}

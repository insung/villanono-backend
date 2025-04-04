using OpenSearch.Client;

public class StatisticalSummary
{
    public StatisticsSummaryTotal Total { get; set; }
    public List<StatisticsSummaryItem> Items { get; set; }

    public StatisticalSummary(
        int beginDate,
        int endDate,
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

        Items = new List<StatisticsSummaryItem>();

        var begin = ConvertDateOnly(beginDate);
        var end = ConvertDateOnly(endDate);

        for (DateOnly date = begin; date <= end; date = date.AddDays(1))
        {
            var intDate = ConvertInt32(date);
            var whereCondition = contractDateBuckets.Buckets.Where(b => b.Key == intDate);
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
                Items.Add(new StatisticsSummaryItem { ContractDate = intDate });
            }
        }

        Total = new StatisticsSummaryTotal(
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

    private DateOnly ConvertDateOnly(int date)
    {
        var year = date / 10000;
        var month = (date / 100) % 100;
        var day = date % 100;

        return new DateOnly(year, month, day);
    }

    private int ConvertInt32(DateOnly date)
    {
        var intDate = date.Year * 10000 + date.Month * 100 + date.Day;
        return intDate;
    }
}

public class StatisticsSummaryBase
{
    public long Count { get; set; }
    public double? Average { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double Sum { get; set; }
    public double? Percentiles25 { get; set; }
    public double? Percentiles50 { get; set; }
    public double? Percentiles75 { get; set; }
}

public class StatisticsSummaryTotal : StatisticsSummaryBase
{
    public double StdDev { get; set; }

    public StatisticsSummaryTotal(
        long count,
        double? average,
        double? min,
        double? max,
        double sum,
        double? percentiles25,
        double? percentiles50,
        double? percentiles75,
        IEnumerable<double?> itemsAverage
    )
    {
        Count = count;
        Average = average;
        Min = min;
        Max = max;
        Sum = sum;
        Percentiles25 = percentiles25;
        Percentiles50 = percentiles50;
        Percentiles75 = percentiles75;

        double varianceSum = 0.0;
        foreach (var itemAverage in itemsAverage)
        {
            varianceSum += Math.Pow(itemAverage ?? 0 - Average ?? 0, 2);
        }
        double variance = varianceSum / Count;
        StdDev = Math.Sqrt(variance);
    }
}

public class StatisticsSummaryItem : StatisticsSummaryBase
{
    public int ContractDate { get; set; }
}

public class StatisticalSummary
{
    public long TotalCount { get; set; }
    public double TotalAverage { get; set; }
    public double StdDev { get; set; }
    public List<StatisticsSummaryItem> Items { get; set; }

    public StatisticalSummary(ESStatisticsSummaryResponse response)
    {
        TotalCount = response.TotalCount;
        TotalAverage = response.TotalAverage;
        StdDev = response.StdDev;

        Items = new List<StatisticsSummaryItem>();
        response.Key.Buckets.ForEach(x =>
            Items.Add(
                new StatisticsSummaryItem
                {
                    ContractYearMonth = x.Key,
                    Count = x.Count,
                    Average = x.Average,
                    Min = x.Min,
                    Max = x.Max,
                    Percentiles25 = x.Percentiles25,
                    Percentiles50 = x.Percentiles50,
                    Percentiles75 = x.Percentiles75,
                }
            )
        );
    }
}

public class StatisticsSummaryItem
{
    public int ContractYearMonth { get; set; }
    public long Count { get; set; }
    public double Average { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Percentiles25 { get; set; }
    public double Percentiles50 { get; set; }
    public double Percentiles75 { get; set; }
}

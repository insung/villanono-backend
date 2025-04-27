public class InsightReportBaseModel
{
    public InsightSummary Total { get; set; }
    public List<InsightSummaryItem> Items { get; set; }

    protected DateOnly ConvertDateOnly(int date)
    {
        var year = date / 10000;
        var month = (date / 100) % 100;
        var day = date % 100;

        return new DateOnly(year, month, day);
    }

    protected int ConvertInt32(DateOnly date)
    {
        var intDate = date.Year * 10000 + date.Month * 100 + date.Day;
        return intDate;
    }

    protected DateOnly ConvertBeginDate(int beginYearMonth)
    {
        var year = beginYearMonth / 100;
        var month = beginYearMonth % 100;
        return new DateOnly(year, month, 1);
    }

    protected DateOnly ConvertEndDate(int endYearMonth)
    {
        var year = endYearMonth / 100;
        var month = endYearMonth % 100;
        return new DateOnly(year, month, DateTime.DaysInMonth(year, month));
    }
}

public class InsightSummaryBase
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

public class InsightSummary : InsightSummaryBase
{
    public double StdDev { get; set; }

    public InsightSummary(
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

public class InsightSummaryItem : InsightSummaryBase
{
    public int ContractDate { get; set; }
}

public class ReportService : IReportService
{
    readonly IVillanonoRepository villanonoRepository;

    public ReportService(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
    }

    public async Task<InsightReportDailyModel> GetInsightDaily(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        return await villanonoRepository.GetReportInsightDaily(
            dataType,
            beginDate,
            endDate,
            dong,
            gu,
            si
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
        int? constructionYear = null
    )
    {
        return await villanonoRepository.GetReportInsightMonthly(
            dataType,
            beginYearMonth,
            endYearMonth,
            dong,
            gu,
            si,
            exclusiveAreaBegin,
            exclusiveAreaEnd,
            constructionYear
        );
    }
}

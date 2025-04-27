public interface IReportService
{
    /// <summary>
    /// Daily Insight 리포트 데이터 가져오는 메서드
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="beginDate"></param>
    /// <param name="endDate"></param>
    /// <param name="dong"></param>
    /// <param name="gu"></param>
    /// <param name="si"></param>
    /// <returns></returns>
    Task<InsightReportDailyModel> GetInsightDaily(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    );

    /// <summary>
    /// Monthly Insight 리포트 데이터 가져오는 메서드
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="beginYearMonth"></param>
    /// <param name="endYearMonth"></param>
    /// <param name="dong"></param>
    /// <param name="gu"></param>
    /// <param name="si"></param>
    /// <returns></returns>
    Task<InsightReportMonthlyModel> GetInsightMonthly(
        VillanonoDataType dataType,
        int beginYearMonth,
        int endYearMonth,
        string dong,
        string gu,
        string si = "서울특별시"
    );
}

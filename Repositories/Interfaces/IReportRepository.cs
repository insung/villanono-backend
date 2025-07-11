public interface IReportRepository
{
    /// <summary>
    /// Daily Insight 리포트 데이터를 가져오는 메서드
    /// </summary>
    /// <param name="dataType">데이터 타입 (매매/임대)</param>
    /// <param name="beginDate">계약일자 시작일 (yyyy-MM-dd)</param>
    /// <param name="endDate">계약일자 종료일 (yyyy-MM-dd)</param>
    /// <param name="si">시</param>
    /// <param name="gu">구</param>
    /// <param name="dong">동</param>
    /// <param name="beginExclusiveArea">전용면적 시작 (m²)</param>
    /// <param name="endExclusiveArea">전용면적 종료 (m²)</param>
    /// <param name="beginConstructYear">건축년도 시작</param>
    /// <param name="endConstructYear">건축년도 종료</param>
    /// <param name="indexName">검색할 인덱스 이름 패턴</param>
    /// <returns>일별 통계 리포트 데이터</returns>
    Task<InsightReportDailyModel> GetInsightDaily(
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
    );

    /// <summary>
    /// Monthly Insight 리포트 데이터를 가져오는 메서드
    /// </summary>
    /// <param name="dataType">데이터 타입 (매매/임대)</param>
    /// <param name="beginYearMonth">시작년월 (yyyyMM)</param>
    /// <param name="endYearMonth">종료년월 (yyyyMM)</param>
    /// <param name="si">시</param>
    /// <param name="gu">구</param>
    /// <param name="dong">동</param>
    /// <param name="beginExclusiveArea">전용면적 시작 (m²)</param>
    /// <param name="endExclusiveArea">전용면적 종료 (m²)</param>
    /// <param name="beginConstructYear">건축년도 시작</param>
    /// <param name="endConstructYear">건축년도 종료</param>
    /// <param name="indexName">검색할 인덱스 이름 패턴</param>
    /// <returns>월별 통계 리포트 데이터</returns>
    Task<InsightReportMonthlyModel> GetInsightMonthly(
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
    );
}

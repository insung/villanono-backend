using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    readonly IReportRepository reportRepository;

    public ReportController(IReportRepository reportRepository)
    {
        this.reportRepository = reportRepository;
    }

    /// <summary>
    /// Daily Insight 통계 데이터 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginDate">시작일 (yyyy-MM-dd)</param>
    /// <param name="endDate">종료일 (yyyy-MM-dd)</param>
    /// <param name="dong">동</param>
    /// <param name="gu">구</param>
    /// <param name="si">시</param>
    /// <returns>통계데이터</returns>
    [HttpPost("Insight/Daily")]
    public async Task<IActionResult> GetInsightDaily(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        var dailyInsightData = await reportRepository.GetInsightDaily(
            dataType,
            beginDate,
            endDate,
            dong,
            gu,
            si
        );
        return Ok(dailyInsightData);
    }

    /// <summary>
    /// Monthly Insight 통계 데이터 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginYearMonth">시작년월 (yyyyMM)</param>
    /// <param name="endYearMonth">종료년월 (yyyyMM)</param>
    /// <param name="dong">동</param>
    /// <param name="gu">구</param>
    /// <param name="si">시</param>
    /// <param name="exclusiveAreaBegin">전용면적 시작</param>
    /// <param name="exclusiveAreaEnd">전용면적 종료</param>
    /// <param name="constructionYear">해당 건축년도 이상</param>
    /// <returns>통계데이터</returns>
    [HttpPost("Insight/Monthly")]
    public async Task<IActionResult> GetInsightMonthly(
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
        var dailyInsightData = await reportRepository.GetInsightMonthly(
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
        return Ok(dailyInsightData);
    }
}

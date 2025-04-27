using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    readonly IReportService reportService;

    public ReportController(IReportService reportService)
    {
        this.reportService = reportService;
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
    public async Task<IActionResult> GetReportInsightDaily(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        var dailyInsightData = await reportService.GetInsightDaily(
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
    /// <returns>통계데이터</returns>
    [HttpPost("Insight/Monthly")]
    public async Task<IActionResult> GetReportInsightMonthly(
        VillanonoDataType dataType,
        int beginYearMonth,
        int endYearMonth,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        var dailyInsightData = await reportService.GetInsightMonthly(
            dataType,
            beginYearMonth,
            endYearMonth,
            dong,
            gu,
            si
        );
        return Ok(dailyInsightData);
    }
}

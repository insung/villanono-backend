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
    /// <param name="beginDate">계약일자 시작일 (yyyy-MM-dd)</param>
    /// <param name="endDate">계약일자 종료일 (yyyy-MM-dd)</param>
    /// <param name="si">시</param>
    /// <param name="gu">구</param>
    /// <param name="dong">동</param>
    /// <param name="beginExclusiveArea">전용면적 시작 (m²)</param>
    /// <param name="endExclusiveArea">전용면적 종료 (m²)</param>
    /// <param name="beginConstructYear">건축년도 시작</param>
    /// <param name="endConstructYear">건축년도 종료</param>
    /// <returns>일별 통계 데이터</returns>
    [HttpPost("Insight/Daily")]
    public async Task<IActionResult> GetInsightDaily(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null,
        int? beginConstructYear = null,
        int? endConstructYear = null
    )
    {
        var dailyInsightData = await reportRepository.GetInsightDaily(
            dataType,
            beginDate,
            endDate,
            si,
            gu,
            dong,
            beginExclusiveArea,
            endExclusiveArea,
            beginConstructYear,
            endConstructYear
        );
        return Ok(dailyInsightData);
    }

    /// <summary>
    /// Monthly Insight 통계 데이터 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginYearMonth">시작년월 (yyyyMM)</param>
    /// <param name="endYearMonth">종료년월 (yyyyMM)</param>
    /// <param name="si">시</param>
    /// <param name="gu">구</param>
    /// <param name="dong">동</param>
    /// <param name="beginExclusiveArea">전용면적 시작 (m²)</param>
    /// <param name="endExclusiveArea">전용면적 종료 (m²)</param>
    /// <param name="beginConstructYear">건축년도 시작</param>
    /// <param name="endConstructYear">건축년도 종료</param>
    /// <returns>월별 통계 데이터</returns>
    [HttpPost("Insight/Monthly")]
    public async Task<IActionResult> GetInsightMonthly(
        VillanonoDataType dataType,
        int beginYearMonth,
        int endYearMonth,
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null,
        int? beginConstructYear = null,
        int? endConstructYear = null
    )
    {
        var dailyInsightData = await reportRepository.GetInsightMonthly(
            dataType,
            beginYearMonth,
            endYearMonth,
            si,
            gu,
            dong,
            beginExclusiveArea,
            endExclusiveArea,
            beginConstructYear,
            endConstructYear
        );
        return Ok(dailyInsightData);
    }
}

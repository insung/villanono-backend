using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatisticController : ControllerBase
{
    readonly IDataService villanonoDataService;

    public StatisticController(IDataService villanonoDataService)
    {
        this.villanonoDataService = villanonoDataService;
    }

    /// <summary>
    /// 통계 데이터 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginDate">시작일</param>
    /// <param name="endDate">종료일</param>
    /// <param name="dong">동</param>
    /// <param name="gu">구</param>
    /// <param name="si">시</param>
    /// <returns>통계데이터</returns>
    [HttpGet("StatisticalSummary/{dataType}")]
    public async Task<IActionResult> GetVolumeAndAverage(
        VillanonoDataType dataType,
        [FromQuery] DateOnly beginDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] string dong,
        [FromQuery] string gu,
        [FromQuery] string si = "서울특별시"
    )
    {
        var statisticsSummary = await villanonoDataService.GetStatisticsSummary(
            dataType,
            beginDate,
            endDate,
            dong,
            gu,
            si
        );
        return Ok(statisticsSummary);
    }
}

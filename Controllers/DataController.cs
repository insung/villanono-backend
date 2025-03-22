using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    readonly IDataService villanonoDataService;

    public DataController(IDataService villanonoDataService)
    {
        this.villanonoDataService = villanonoDataService;
    }

    [HttpGet("RawData/{dataType}")]
    public async Task<IActionResult> GetData(
        VillanonoDataType dataType,
        [FromQuery] int beginDate,
        [FromQuery] int endDate,
        [FromQuery] string dong,
        [FromQuery] string gu,
        [FromQuery] string si = "서울특별시"
    )
    {
        var models = await villanonoDataService.GetData(dataType, beginDate, endDate, dong, gu, si);
        return Ok(models);
    }

    [HttpGet("StatisticalSummary/{dataType}")]
    public async Task<IActionResult> GetVolumeAndAverage(
        VillanonoDataType dataType,
        [FromQuery] int beginDate,
        [FromQuery] int endDate,
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

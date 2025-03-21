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
        if (dataType == VillanonoDataType.BuySell)
        {
            var buySellModels = await villanonoDataService.GetData<BuySellModel>(
                beginDate,
                endDate,
                dong,
                gu,
                si
            );
            return Ok(buySellModels);
        }
        else
        {
            return Ok();
        }
    }
}

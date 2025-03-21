using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BuySellController : ControllerBase
{
    readonly IVillanonoDataService villanonoDataService;

    public BuySellController(IVillanonoDataService villanonoDataService)
    {
        this.villanonoDataService = villanonoDataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBuySell(
        [FromQuery] int beginDate,
        [FromQuery] int endDate,
        [FromQuery] string dong,
        [FromQuery] string gu,
        [FromQuery] string si = "서울특별시"
    )
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
}

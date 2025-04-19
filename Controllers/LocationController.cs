using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    readonly ILocationService locationService;

    public LocationController(ILocationService locationService)
    {
        this.locationService = locationService;
    }

    /// <summary>
    /// "시" 목록을 가져오는 API
    /// </summary>
    /// <returns></returns>
    [HttpGet("Si")]
    public async Task<IActionResult> GetAllSi()
    {
        return Ok();
    }

    /// <summary>
    /// 선택한 "시"에 대한 "구" 목록을 가져오는 API
    /// </summary>
    /// <param name="Si">시 이름</param>
    /// <returns></returns>
    [HttpGet("Si/{Si}/Gu")]
    public async Task<IActionResult> GetAllGu([FromRoute] string Si)
    {
        return Ok();
    }

    /// <summary>
    /// 선택한 "시"와 "구"에 대한 "동" 목록을 가져오는 API
    /// </summary>
    /// <param name="Si">시 이름</param>
    /// <param name="Gu">구 이름</param>
    /// <returns></returns>
    [HttpGet("Si/{Si}/Gu/{Gu}/Dong")]
    public async Task<IActionResult> GetAllDong([FromRoute] string Si, [FromRoute] string Gu)
    {
        return Ok();
    }

    /// <summary>
    /// 빌라노노 위치정보 Bulk Insert
    /// </summary>
    /// <param name="files"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    [HttpPost("BulkInsert")]
    public async Task<IActionResult> BulkInsertLocations(
        List<IFormFile> files,
        VillanonoDataType dataType
    )
    {
        foreach (var csvFile in files)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var stream = csvFile.OpenReadStream();

            if (dataType == VillanonoDataType.BuySell)
                await locationService.BulkInsertLocations<BuySellModel>(stream);
            else
                await locationService.BulkInsertLocations<RentModel>(stream);
        }

        return Ok();
    }
}

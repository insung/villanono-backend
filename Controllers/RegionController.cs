using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RegionController : ControllerBase
{
    readonly ILocationRepository locationRepository;
    readonly ILocationService locationService;

    public RegionController(
        ILocationRepository locationRepository,
        ILocationService locationService
    )
    {
        this.locationRepository = locationRepository;
        this.locationService = locationService;
    }

    /// <summary>
    /// "시" 목록을 가져오는 API
    /// </summary>
    /// <returns></returns>
    [HttpGet("Si")]
    public async Task<IActionResult> GetAllSi()
    {
        var result = await locationRepository.GetAllSi();
        return Ok(result);
    }

    /// <summary>
    /// 선택한 "시"에 대한 "구" 목록을 가져오는 API
    /// </summary>
    /// <param name="si">시 이름</param>
    /// <returns></returns>
    [HttpGet("Si/{si}/Gu")]
    public async Task<IActionResult> GetAllGu([FromRoute] string si)
    {
        var result = await locationRepository.GetAllGu(si);
        return Ok(result);
    }

    /// <summary>
    /// 선택한 "시"와 "구"에 대한 "동" 목록을 가져오는 API
    /// </summary>
    /// <param name="si">시 이름</param>
    /// <param name="gu">구 이름</param>
    /// <returns></returns>
    [HttpGet("Si/{si}/Gu/{gu}/Dong")]
    public async Task<IActionResult> GetAllDong([FromRoute] string si, [FromRoute] string gu)
    {
        var result = await locationRepository.GetAllDong(si, gu);
        return Ok(result);
    }

    /// <summary>
    /// 시, 구, 동 정보 Bulk Insert
    /// </summary>
    /// <param name="files"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    [HttpPost("BulkInsert")]
    public async Task<IActionResult> BulkInsertRegions(
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
                await locationService.BulkInsertRegions<BuySellModel>(stream);
            else
                await locationService.BulkInsertRegions<RentModel>(stream);
        }

        return Ok();
    }
}

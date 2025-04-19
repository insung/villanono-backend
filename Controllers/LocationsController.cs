using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    readonly IDataService villanonoDataService;

    public LocationsController(IDataService villanonoDataService)
    {
        this.villanonoDataService = villanonoDataService;
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
}

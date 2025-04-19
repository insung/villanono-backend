using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RawDataController : ControllerBase
{
    readonly IDataService villanonoDataService;

    public RawDataController(IDataService villanonoDataService)
    {
        this.villanonoDataService = villanonoDataService;
    }

    /// <summary>
    /// RawData 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginDate">시작일</param>
    /// <param name="endDate">종료일</param>
    /// <param name="dong">동</param>
    /// <param name="gu">구</param>
    /// <param name="si">시</param>
    /// <returns></returns>
    [HttpGet("RawData/{dataType}")]
    public async Task<IActionResult> GetData(
        VillanonoDataType dataType,
        [FromQuery] DateOnly beginDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] string dong,
        [FromQuery] string gu,
        [FromQuery] string si = "서울특별시"
    )
    {
        var models = await villanonoDataService.GetData(dataType, beginDate, endDate, dong, gu, si);
        return Ok(models);
    }
}

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IndexManagementController : ControllerBase
{
    readonly IIndexManagementService indexManagementService;

    public IndexManagementController(IIndexManagementService indexManagementService)
    {
        this.indexManagementService = indexManagementService;
    }

    /// <summary>
    /// 살아있는지 확인
    /// </summary>
    /// <returns></returns>
    [HttpGet("HealthCheck")]
    public async Task<IActionResult> HealthCheck()
    {
        await indexManagementService.HealthCheck();
        return Ok();
    }

    [Obsolete(
        "Default Index 가 만들어지게 되면 Field 들이 없기 때문에 추후 입력될 Index 들과의 충돌이 발생할 수 있기 때문에 Deprecated 되었습니다."
    )]
    [HttpPost("CreateDefaultIndex")]
    public IActionResult CreateDefaultIndex()
    {
        return Ok();
    }

    /// <summary>
    /// 인덱스 생성
    /// </summary>
    /// <param name="indexFullName"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    [HttpPost("{indexFullName}/{dataType}")]
    public async Task<IActionResult> CreateIndex(string indexFullName, VillanonoDataType dataType)
    {
        if (dataType == VillanonoDataType.BuySell)
            await indexManagementService.CreateIndex<BuySellModel>(indexFullName);
        else if (dataType == VillanonoDataType.Rent)
            await indexManagementService.CreateIndex<RentModel>(indexFullName);
        return Ok();
    }

    /// <summary>
    /// 인덱스 삭제
    /// </summary>
    /// <param name="indexFullName"></param>
    /// <returns></returns>
    [HttpDelete("{indexFullName}")]
    public async Task<IActionResult> DeleteIndex(string indexFullName)
    {
        await indexManagementService.DeleteIndex(indexFullName);
        return Ok();
    }
}

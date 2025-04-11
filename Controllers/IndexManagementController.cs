using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IndexManagementController : ControllerBase
{
    readonly IRepositoryService repositoryService;

    public IndexManagementController(IRepositoryService repositoryService)
    {
        this.repositoryService = repositoryService;
    }

    /// <summary>
    /// Repository 살아있는지 확인
    /// </summary>
    /// <returns></returns>
    [HttpGet("Ping")]
    public async Task<IActionResult> Ping()
    {
        await repositoryService.HealthCheck();
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
            await repositoryService.CreateIndex<BuySellModel>(indexFullName);
        else if (dataType == VillanonoDataType.Rent)
            await repositoryService.CreateIndex<RentModel>(indexFullName);
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
        await repositoryService.DeleteIndex(indexFullName);
        return Ok();
    }
}

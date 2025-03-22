using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RepositoryController : ControllerBase
{
    readonly IRepositoryService repositoryService;

    public RepositoryController(IRepositoryService repositoryService)
    {
        this.repositoryService = repositoryService;
    }

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
    public async Task<IActionResult> CreateDefaultIndex()
    {
        await repositoryService.CreateIndex("villanono");
        return Ok();
    }

    [HttpPost("BulkInsert/{dataType}/{indexNameSuffix}")]
    public async Task<IActionResult> PutBuySellData(
        IFormFile csvFile,
        VillanonoDataType dataType,
        int indexNameSuffix
    )
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var stream = csvFile.OpenReadStream();
        var totalRowAffected = 0;
        var indexName = $"villanono-{indexNameSuffix}";

        if (dataType == VillanonoDataType.BuySell)
            totalRowAffected = await repositoryService.BulkInsert<BuySellModel>(stream, indexName);
        else
            totalRowAffected = await repositoryService.BulkInsert<RentModel>(stream, indexName);

        return Ok($"{totalRowAffected} row affected");
    }

    [HttpPost("{indexName}")]
    public async Task<IActionResult> CreateIndex(string indexName)
    {
        await repositoryService.CreateIndex(indexName);
        return Ok();
    }

    [HttpDelete("{indexName}")]
    public async Task<IActionResult> DeleteIndex(string indexName)
    {
        await repositoryService.DeleteIndex(indexName);
        return Ok();
    }
}

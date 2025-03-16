using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VillanonoRepositoryController : ControllerBase
{
    readonly IVillanonoLoadService villanonoDataService;

    public VillanonoRepositoryController(IVillanonoLoadService villanonoDataService)
    {
        this.villanonoDataService = villanonoDataService;
    }

    [HttpGet("Ping")]
    public async Task<IActionResult> Ping()
    {
        await villanonoDataService.RepositoryHealthCheck();
        return Ok();
    }

    [HttpPost("CreateDefaultIndex")]
    public async Task<IActionResult> CreateDefaultIndex()
    {
        await villanonoDataService.CreateIndex("villanono");
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
            totalRowAffected = await villanonoDataService.BulkInsert<BuySellModel>(
                stream,
                indexName
            );
        else
            totalRowAffected = await villanonoDataService.BulkInsert<RentModel>(stream, indexName);

        return Ok($"{totalRowAffected} row affected");
    }

    [HttpPost("{indexName}")]
    public async Task<IActionResult> CreateIndex(string indexName)
    {
        await villanonoDataService.CreateIndex(indexName);
        return Ok();
    }

    [HttpDelete("{indexName}")]
    public async Task<IActionResult> DeleteIndex(string indexName)
    {
        await villanonoDataService.DeleteIndex(indexName);
        return Ok();
    }
}

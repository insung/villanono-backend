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
    public IActionResult CreateDefaultIndex()
    {
        return Ok();
    }

    [HttpPost("BulkInsertData")]
    public async Task<IActionResult> BulkInsertData([FromForm] FileUploadModel fileUploadModels)
    {
        var resultMsg = new List<string>();
        var dataType = fileUploadModels.DataType;

        for (int index = 0; index < fileUploadModels.Files.Count; index++)
        {
            var csvFile = fileUploadModels.Files[index];
            var yyyyMMdd = fileUploadModels.FileNames[index];

            if (csvFile == null || csvFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var stream = csvFile.OpenReadStream();
            var totalRowAffected = 0;
            var indexName = $"villanono-{dataType.ToString().ToLower()}-{yyyyMMdd}";

            if (dataType == VillanonoDataType.BuySell)
                totalRowAffected = await repositoryService.BulkInsertData<BuySellModel>(
                    stream,
                    indexName
                );
            else
                totalRowAffected = await repositoryService.BulkInsertData<RentModel>(
                    stream,
                    indexName
                );

            resultMsg.Add(
                $"Successfully processed {totalRowAffected} records in the index '{indexName}'."
            );
        }

        return Ok(resultMsg);
    }

    [HttpPost("BulkInsertLocations")]
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
                await repositoryService.BulkInsertLocations<BuySellModel>(stream);
            else
                await repositoryService.BulkInsertLocations<RentModel>(stream);
        }

        return Ok();
    }

    [HttpPost("{indexFullName}/{dataType}")]
    public async Task<IActionResult> CreateIndex(string indexFullName, VillanonoDataType dataType)
    {
        if (dataType == VillanonoDataType.BuySell)
            await repositoryService.CreateIndex<BuySellModel>(indexFullName);
        else if (dataType == VillanonoDataType.Rent)
            await repositoryService.CreateIndex<RentModel>(indexFullName);
        return Ok();
    }

    [HttpDelete("{indexFullName}")]
    public async Task<IActionResult> DeleteIndex(string indexFullName)
    {
        await repositoryService.DeleteIndex(indexFullName);
        return Ok();
    }
}

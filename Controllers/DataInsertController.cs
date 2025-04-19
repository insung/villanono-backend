using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataInsertController : ControllerBase
{
    readonly IRepositoryService repositoryService;

    public DataInsertController(IRepositoryService repositoryService)
    {
        this.repositoryService = repositoryService;
    }

    /// <summary>
    /// 빌라노노 데이터 Bulk Insert (위치정보도 갱신됨)
    /// </summary>
    /// <param name="fileUploadModels"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 빌라노노 위치정보 Bulk Insert
    /// </summary>
    /// <param name="files"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
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
}

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    readonly IDataService dataService;

    public DataController(IDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <summary>
    /// 빌라노노 데이터 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginDate">시작일 (yyyy-MM-dd)</param>
    /// <param name="endDate">종료일 (yyyy-MM-dd)</param>
    /// <param name="dong">동</param>
    /// <param name="gu">구</param>
    /// <param name="si">시</param>
    /// <returns></returns>
    [HttpGet("{dataType}")]
    public async Task<IActionResult> GetData(
        VillanonoDataType dataType,
        [FromQuery] DateOnly beginDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] string dong,
        [FromQuery] string gu,
        [FromQuery] string si = "서울특별시"
    )
    {
        var models = await dataService.GetData(dataType, beginDate, endDate, dong, gu, si);
        return Ok(models);
    }

    /// <summary>
    /// 빌라노노 데이터 Bulk Insert (위치정보도 갱신됨)
    /// </summary>
    /// <param name="fileUploadModels"></param>
    /// <returns></returns>
    [HttpPost("BulkInsert")]
    public async Task<IActionResult> BulkInsert([FromForm] FileUploadModel fileUploadModels)
    {
        var resultMsg = new List<string>();
        var dataType = fileUploadModels.DataType;

        for (int index = 0; index < fileUploadModels.Files.Count; index++)
        {
            var csvFile = fileUploadModels.Files[index];
            var yyyyMMdd = fileUploadModels.IndexNames[index];

            if (csvFile == null || csvFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var stream = csvFile.OpenReadStream();
            var totalRowAffected = 0;
            var indexName = $"villanono-{dataType.ToString().ToLower()}-{yyyyMMdd}";

            if (dataType == VillanonoDataType.BuySell)
                totalRowAffected = await dataService.BulkInsertData<BuySellModel>(
                    stream,
                    indexName
                );
            else
                totalRowAffected = await dataService.BulkInsertData<RentModel>(stream, indexName);

            resultMsg.Add(
                $"Successfully processed {totalRowAffected} records in the index '{indexName}'."
            );
        }

        return Ok(resultMsg);
    }
}

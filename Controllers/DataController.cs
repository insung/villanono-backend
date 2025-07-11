using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    readonly IDataService dataService;
    readonly IDataRepository dataRepository;

    public DataController(IDataService dataService, IDataRepository dataRepository)
    {
        this.dataService = dataService;
        this.dataRepository = dataRepository;
    }

    /// <summary>
    /// 빌라노노 데이터 가져오기
    /// </summary>
    /// <param name="dataType">데이터타입</param>
    /// <param name="beginContractDate">계약일자 시작일 (yyyy-MM-dd)</param>
    /// <param name="endContractDate">계약일자 종료일 (yyyy-MM-dd)</param>
    /// <param name="beginTransactionAmount"></param>
    /// <param name="endTransactionAmount"></param>
    /// <param name="constructYear"></param>
    /// <param name="exclusiveArea"></param>
    /// <param name="dong">동</param>
    /// <param name="gu">구</param>
    /// <param name="si">시</param>
    /// <returns></returns>
    [HttpGet("{dataType}")]
    public async Task<IActionResult> GetData(
        VillanonoDataType dataType,
        [FromQuery] string si = "서울특별시",
        [FromQuery] string? dong = null,
        [FromQuery] string? gu = null,
        [FromQuery] DateOnly? beginContractDate = null,
        [FromQuery] DateOnly? endContractDate = null,
        [FromQuery] double? beginTransactionAmount = null,
        [FromQuery] double? endTransactionAmount = null,
        [FromQuery] int? beginConstructYear = null,
        [FromQuery] int? endConstructYear = null,
        [FromQuery] double? beginExclusiveArea = null,
        [FromQuery] double? endExclusiveArea = null
    )
    {
        if (dataType == VillanonoDataType.BuySell)
        {
            var models = await dataRepository.SearchBySiGuDong<BuySellModel>(
                new HashSet<VillanonoDataType> { dataType },
                si,
                gu,
                dong,
                beginContractDate,
                endContractDate,
                beginTransactionAmount,
                endTransactionAmount,
                beginConstructYear,
                endConstructYear,
                beginExclusiveArea,
                endExclusiveArea
            );
            return Ok(models);
        }
        else if (dataType == VillanonoDataType.Rent)
        {
            var models = await dataRepository.SearchBySiGuDong<RentModel>(
                new HashSet<VillanonoDataType> { dataType },
                si,
                gu,
                dong,
                beginContractDate,
                endContractDate,
                beginTransactionAmount,
                endTransactionAmount,
                beginConstructYear,
                endConstructYear,
                beginExclusiveArea,
                endExclusiveArea
            );
            return Ok(models);
        }
        else
        {
            throw new ArgumentException("Invalid dataType");
        }
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

    [HttpGet("SearchByRoadName")]
    public async Task<IActionResult> SearchByRoadName(
        [FromQuery] string roadName,
        [FromQuery] string buildingName,
        [FromQuery] int? greaterThanContractDate
    )
    {
        var models = await dataRepository.SearchByRoadName<BuySellModel>(
            roadName,
            buildingName,
            greaterThanContractDate
        );
        return Ok(models);
    }
}

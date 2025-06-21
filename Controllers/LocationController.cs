using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    readonly ILocationRepository locationRepository;
    readonly ILocationService locationService;
    readonly IJobQueue jobQueue;

    public LocationController(
        ILocationRepository locationRepository,
        ILocationService locationService,
        IJobQueue jobQueue
    )
    {
        this.locationRepository = locationRepository;
        this.locationService = locationService;
        this.jobQueue = jobQueue;
    }

    /// <summary>
    /// "시" 목록을 가져오는 API
    /// </summary>
    /// <returns></returns>
    [HttpGet("Si")]
    public async Task<IActionResult> GetAllSi()
    {
        var result = await locationRepository.GetAllSi();
        return Ok(result);
    }

    /// <summary>
    /// 선택한 "시"에 대한 "구" 목록을 가져오는 API
    /// </summary>
    /// <param name="si">시 이름</param>
    /// <returns></returns>
    [HttpGet("Si/{Si}/Gu")]
    public async Task<IActionResult> GetAllGu([FromRoute] string si)
    {
        var result = await locationRepository.GetAllGu(si);
        return Ok(result);
    }

    /// <summary>
    /// 선택한 "시"와 "구"에 대한 "동" 목록을 가져오는 API
    /// </summary>
    /// <param name="si">시 이름</param>
    /// <param name="gu">구 이름</param>
    /// <returns></returns>
    [HttpGet("Si/{Si}/Gu/{Gu}/Dong")]
    public async Task<IActionResult> GetAllDong([FromRoute] string si, [FromRoute] string gu)
    {
        var result = await locationRepository.GetAllDong(si, gu);
        return Ok(result);
    }

    /// <summary>
    /// 빌라노노 위치정보 Bulk Insert
    /// </summary>
    /// <param name="files"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    [HttpPost("BulkInsert")]
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
                await locationService.BulkInsertLocations<BuySellModel>(stream);
            else
                await locationService.BulkInsertLocations<RentModel>(stream);
        }

        return Ok();
    }

    /// <summary>
    /// "시", "구", "도로명"에 대한 주소 정보를 가져오는 API
    /// </summary>
    /// <param name="gu"></param>
    /// <param name="roadName"></param>
    /// <param name="si"></param>
    /// <returns></returns>
    [HttpGet("GetAddress")]
    public async Task<IActionResult> GetAddress(
        [FromQuery] string gu = "",
        [FromQuery] string roadName = "",
        [FromQuery] string si = "서울특별시"
    )
    {
        var addressList = await locationRepository.GetAddress(si, gu, roadName);
        return Ok(addressList);
    }

    /// <summary>
    /// 지오코드 정보의 개수를 반환하는 API
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetGeocodeCount")]
    public async Task<IActionResult> GetGeocodeCount()
    {
        var count = await locationRepository.GetGeocodeCount();
        return Ok(count);
    }

    /// <summary>
    /// 지오코드 정보를 인덱스에 삽입하는 API
    /// </summary>
    /// <param name="si"></param>
    /// <param name="vWorldAPIRequestQuota"></param>
    /// <returns></returns>
    [HttpPost("StartInsertGeocode")]
    public async Task<IActionResult> StartInsertGeocode(
        [FromQuery] string si = "서울특별시",
        [FromQuery] int vWorldAPIRequestQuota = 1000
    )
    {
        await jobQueue.EnqueueAsync(
            async (serviceProvider, cancellationToken) =>
            {
                // 이 람다 식 안에서 필요한 서비스들을 가져와 사용합니다.
                var repository = serviceProvider.GetRequiredService<ILocationRepository>();
                var service = serviceProvider.GetRequiredService<ILocationService>();

                var addressList = await repository.GetAddress(si);
                await service.BulkInsertGeocode(addressList, vWorldAPIRequestQuota);
            }
        );

        return Accepted("Job was queued successfully.");
    }
}

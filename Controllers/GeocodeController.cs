using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GeocodeController : ControllerBase
{
    readonly ILocationRepository locationRepository;
    readonly IJobQueue jobQueue;

    public GeocodeController(ILocationRepository locationRepository, IJobQueue jobQueue)
    {
        this.locationRepository = locationRepository;
        this.jobQueue = jobQueue;
    }

    /// <summary>
    /// "시", "구", "검색"에 대한 주소 정보(도로명, 지번)를 가져오는 API
    /// </summary>
    /// <param name="si"></param>
    /// <param name="gu"></param>
    /// <param name="search"></param>
    /// <param name="addressType"></param>
    /// <returns></returns>
    [HttpGet("Geocode")]
    public async Task<IActionResult> GetGeocode(
        [FromQuery] string si = "서울특별시",
        [FromQuery] string gu = "",
        [FromQuery] string search = "",
        [FromQuery] AddressType addressType = AddressType.Road
    )
    {
        // var strategy = new AddressModelQueryStrategy();
        // var geocodeList = await locationRepository.GetAddress(
        //     strategy,
        //     si,
        //     gu,
        //     search,
        //     addressType
        // );
        // return Ok(geocodeList);
        var strategy = new GeocodeModelQueryStrategy();
        var geocodeList = await locationRepository.GetAddress(
            strategy,
            si,
            gu,
            search,
            addressType
        );
        return Ok(geocodeList);
    }

    /// <summary>
    /// 지오코드 정보의 총 개수를 반환하는 API
    /// </summary>
    /// <returns></returns>
    [HttpGet("TotalCount")]
    public async Task<IActionResult> GetTotalCount()
    {
        var count = await locationRepository.GetTotalCount();
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

                var addressList = await repository.GetDistinctAddress(si);
                await service.BulkInsertAddress(addressList, vWorldAPIRequestQuota);
            }
        );

        return Accepted("Job was queued successfully.");
    }
}

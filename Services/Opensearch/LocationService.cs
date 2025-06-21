using Microsoft.Extensions.Options;

public class LocationService : ILocationService
{
    readonly ILocationRepository locationRepository;
    readonly IVillanonoCsvReader villanonoCsvReader;
    readonly int batchSize;
    readonly IVWorldRepository vworldRepository;
    readonly VWorldSettingsModel vworldSettingsModel;
    private const string locationsIndex = "si-gu-dong";
    private const string geocodeIndex = "geocode";
    private const int dailyVWorldAPIQuota = 1000;

    public LocationService(
        ILocationRepository locationRepository,
        IVillanonoCsvReader villanonoCsvReader,
        IOptions<OpensearchSettingsModel> elasticSearchSettings,
        IVWorldRepository vworldRepository,
        IOptions<VWorldSettingsModel> vworldSettingsModel
    )
    {
        this.locationRepository = locationRepository;
        this.villanonoCsvReader = villanonoCsvReader;
        batchSize = elasticSearchSettings.Value.BatchSize;
        this.vworldRepository = vworldRepository;
        this.vworldSettingsModel = vworldSettingsModel.Value;
    }

    public async Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel
    {
        var records = new List<T>();
        using var streamReader = new StreamReader(stream);

        await foreach (var record in villanonoCsvReader.Read<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await locationRepository.BulkInsertLocations(records, locationsIndex);
                records.Clear();
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await locationRepository.BulkInsertLocations(records, locationsIndex);
        }
    }

    public async Task BulkInsertGeocode(IList<AddressModel> addressModels)
    {
        int requestCount = 0;
        foreach (var address in addressModels)
        {
            if (requestCount >= dailyVWorldAPIQuota)
            {
                throw new InvalidOperationException(
                    "일일 VWorld API 호출 한도를 초과했습니다. 나중에 다시 시도해주세요."
                );
            }

            if (
                !await locationRepository.HasGeocode(
                    address.Si,
                    address.Gu,
                    address.RoadName,
                    geocodeIndex
                )
            )
            {
                var geocodeResponse = await vworldRepository.GetCoordinatesAsync(
                    key: vworldSettingsModel.ApiKey,
                    address: $"{address.Gu} {address.RoadName}"
                );

                var point = geocodeResponse?.Response?.Result?.Point;
                double? lat = double.TryParse(point?.Y, out double pLat) ? pLat : null;
                double? lon = double.TryParse(point?.X, out double pLon) ? pLon : null;

                var geoCode = new GeocodeModel(address, lat, lon);

                await locationRepository.UpsertGeocode(geoCode);
                requestCount++;
            }
        }
    }
}

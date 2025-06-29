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

    public async Task BulkInsertRegions<T>(Stream stream)
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

    public async Task BulkInsertAddress(
        IList<AddressModel> addressModels,
        int vWorldAPIRequestQuota = 1000
    )
    {
        int requestCount = 0;
        foreach (var address in addressModels)
        {
            if (requestCount >= vWorldAPIRequestQuota)
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

    public async Task<GeocodeModel?> GetGeocodeWithVWorld(string si, string gu, string roadName)
    {
        var geocode = await locationRepository.GetGeocode<GeocodeModel>(
            si,
            gu,
            roadName,
            geocodeIndex
        );

        if (geocode == null)
        {
            // null 인 경우 주소 정보부터 insert 해야함.
            return null;
        }

        if (geocode.Latitude == null || geocode.Longitude == null)
        {
            var geocodeResponse = await vworldRepository.GetCoordinatesAsync(
                key: vworldSettingsModel.ApiKey,
                address: $"{gu} {roadName}"
            );

            var point = geocodeResponse?.Response?.Result?.Point;
            double? lat = double.TryParse(point?.Y, out double pLat) ? pLat : null;
            double? lon = double.TryParse(point?.X, out double pLon) ? pLon : null;
            var newGeocode = new GeocodeModel(
                new AddressModel(
                    si: geocode.Si,
                    gu: geocode.Gu,
                    dong: geocode.Dong,
                    addressNumber: geocode.AddressNumber,
                    roadName: geocode.RoadName
                ),
                lat,
                lon
            );

            // 가져온 지오코드 정보를 인덱스에 삽입합니다.
            await locationRepository.UpsertGeocode(newGeocode);
            return newGeocode;
        }
        else
        {
            // 기존 지오코드 정보가 유효한 경우
            return geocode;
        }
    }
}

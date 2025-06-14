using Microsoft.Extensions.Options;

public class LocationService : ILocationService
{
    readonly ILocationRepository locationRepository;
    readonly IVillanonoCsvReader villanonoCsvReader;
    readonly int batchSize;
    readonly IVWorldRepository vworldRepository;
    readonly VWorldSettingsModel vworldSettingsModel;
    private const string locationsIndex = "si-gu-dong";
    private const string addressIndex = "address";

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

    public async Task<IList<string>> GetAllSi()
    {
        return await locationRepository.GetAllSi(locationsIndex);
    }

    public async Task<IList<string>> GetAllGu(string Si)
    {
        return await locationRepository.GetAllGu(Si, locationsIndex);
    }

    public async Task<IList<string>> GetAllDong(string Si, string Gu)
    {
        return await locationRepository.GetAllDong(Si, Gu, locationsIndex);
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

    public async Task<IList<AddressModel>> GetAddress(string Si, string Gu, string roadName)
    {
        return await locationRepository.GetAllAddress(Si, Gu, roadName);
    }

    public async Task UpsertGeocodes(IList<AddressModel> addresses)
    {
        foreach (var address in addresses)
        {
            var geocodeResponse = await vworldRepository.GetCoordinatesAsync(
                key: vworldSettingsModel.ApiKey,
                address: $"{address.Gu} {address.RoadName}"
            );
        }
    }
}

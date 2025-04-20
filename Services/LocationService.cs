using Microsoft.Extensions.Options;

public class LocationService : ILocationService
{
    readonly IVillanonoRepository villanonoRepository;
    readonly ICSVReader csvReader;
    readonly int batchSize;

    public LocationService(
        IVillanonoRepository villanonoRepository,
        ICSVReader csvReader,
        IOptions<ElasticSearchSettingsModel> elasticSearchSettings
    )
    {
        this.villanonoRepository = villanonoRepository;
        this.csvReader = csvReader;
        batchSize = elasticSearchSettings.Value.BatchSize;
    }

    public async Task<IList<string>> GetAllSi()
    {
        return await villanonoRepository.GetAllSi();
    }

    public async Task<IList<string>> GetAllGu(string Si)
    {
        return await villanonoRepository.GetAllGu(Si);
    }

    public async Task<IList<string>> GetAllDong(string Si, string Gu)
    {
        return await villanonoRepository.GetAllDong(Si, Gu);
    }

    public async Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel
    {
        var records = new List<T>();
        using var streamReader = new StreamReader(stream);

        await foreach (var record in csvReader.Read<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await villanonoRepository.BulkInsertLocations(records);
                records.Clear();
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await villanonoRepository.BulkInsertLocations(records);
        }
    }
}

using Microsoft.Extensions.Options;

/// <summary>
/// 빌라노노 데이터 서비스
/// </summary>
public class DataService : IDataService
{
    readonly IIndexManagementRepository indexManagementRepository;
    readonly IDataRepository dataRepository;
    readonly ILocationRepository locationRepository;
    readonly IVillanonoCsvReader villanonoCsvReader;
    readonly int batchSize;

    public DataService(
        IIndexManagementRepository indexManagementRepository,
        IDataRepository dataRepository,
        ILocationRepository locationRepository,
        IVillanonoCsvReader villanonoCsvReader,
        IOptions<OpensearchSettingsModel> elasticSearchSettings
    )
    {
        this.indexManagementRepository = indexManagementRepository;
        this.dataRepository = dataRepository;
        this.locationRepository = locationRepository;
        this.villanonoCsvReader = villanonoCsvReader;
        batchSize = elasticSearchSettings.Value.BatchSize;
    }

    public async Task<IReadOnlyCollection<VillanonoBaseModel>> GetData(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        if (dataType == VillanonoDataType.BuySell)
        {
            return await dataRepository.GetData<BuySellModel>(
                dataType,
                beginDate,
                endDate,
                dong,
                gu,
                si
            );
        }
        else if (dataType == VillanonoDataType.Rent)
        {
            return await dataRepository.GetData<RentModel>(
                dataType,
                beginDate,
                endDate,
                dong,
                gu,
                si
            );
        }
        else
        {
            throw new ArgumentException("Invalid dataType");
        }
    }

    public async Task<int> BulkInsertData<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel
    {
        var totalRowAffected = 0;
        var records = new List<T>();

        using var streamReader = new StreamReader(stream);

        if (!await indexManagementRepository.HasIndex(indexName))
            await indexManagementRepository.CreateDataIndex<T>(indexName);

        await foreach (var record in villanonoCsvReader.Read<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await dataRepository.BulkInsertData(records, indexName);
                await locationRepository.BulkInsertLocations(records);
                records.Clear();
                totalRowAffected += batchSize;
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await dataRepository.BulkInsertData(records, indexName);
            await locationRepository.BulkInsertLocations(records);
            totalRowAffected += records.Count;
        }

        return totalRowAffected;
    }
}

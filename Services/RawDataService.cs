using Microsoft.Extensions.Options;

/// <summary>
/// 빌라노노 데이터 서비스
/// </summary>
public class RawDataService : IRawDataService
{
    readonly IVillanonoRepository villanonoRepository;
    readonly ICSVReader csvReader;
    readonly int batchSize;

    public RawDataService(
        IVillanonoRepository villanonoRepository,
        ICSVReader csvReader,
        IOptions<ElasticSearchSettingsModel> elasticSearchSettings
    )
    {
        this.villanonoRepository = villanonoRepository;
        this.csvReader = csvReader;
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
            return await villanonoRepository.GetData<BuySellModel>(
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
            return await villanonoRepository.GetData<RentModel>(
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

        if (!await villanonoRepository.HasIndex(indexName))
            await villanonoRepository.CreateDataIndex<T>(indexName);

        await foreach (var record in csvReader.Read<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await villanonoRepository.BulkInsertData(records, indexName);
                await villanonoRepository.BulkInsertLocations(records);
                records.Clear();
                totalRowAffected += batchSize;
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await villanonoRepository.BulkInsertData(records, indexName);
            await villanonoRepository.BulkInsertLocations(records);
            totalRowAffected += records.Count;
        }

        return totalRowAffected;
    }
}

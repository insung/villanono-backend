using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;

public class RepositoryService : IRepositoryService
{
    readonly IVillanonoRepository villanonoRepository;
    readonly int batchSize;

    public RepositoryService(
        IVillanonoRepository villanonoRepository,
        IOptions<ElasticSearchSettingsModel> elasticSearchSettings
    )
    {
        this.villanonoRepository = villanonoRepository;
        batchSize = elasticSearchSettings.Value.BatchSize;
    }

    public async ValueTask HealthCheck()
    {
        await villanonoRepository.Ping();
    }

    public async ValueTask CreateIndex(string indexName)
    {
        if (!await villanonoRepository.HasIndex(indexName))
            await villanonoRepository.CreateIndex(indexName);
    }

    public async Task<int> BulkInsert<T>(Stream stream, string indexName)
    {
        var totalRowAffected = 0;
        var records = new List<T>();

        using var streamReader = new StreamReader(stream);

        if (!await villanonoRepository.HasIndex(indexName))
            await villanonoRepository.CreateIndex(indexName);

        await foreach (var record in ReadCsvFile<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await villanonoRepository.BulkInsert(records, indexName);
                records.Clear();
                totalRowAffected += batchSize;
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await villanonoRepository.BulkInsert(records);
            totalRowAffected += records.Count;
        }

        return totalRowAffected;
    }

    private static async IAsyncEnumerable<T> ReadCsvFile<T>(StreamReader stream)
    {
        await Task.Yield(); // 비동기 I/O 지원

        var csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            // MissingFieldFound = null,
        };

        using var csv = new CsvReader(stream, csvOptions);
        csv.Context.TypeConverterCache.AddConverter<DateTime?>(new VillanonoDateTimeConverter());
        csv.Context.TypeConverterCache.AddConverter<string?>(new VillanonoStringConverter());
        csv.Context.TypeConverterCache.AddConverter<int?>(new VillanonoIntConverter());

        while (await csv.ReadAsync())
        {
            yield return csv.GetRecord<T>();
        }
    }

    public async ValueTask DeleteIndex(string indexName)
    {
        await villanonoRepository.DeleteIndex(indexName);
    }
}

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;

public class VillanonoLoadService : IVillanonoLoadService
{
    readonly IVillanonoRepository villanonoRepository;
    readonly int batchSize;

    public VillanonoLoadService(
        IVillanonoRepository villanonoRepository,
        IOptions<ElasticSearchSettingsModel> elasticSearchSettings
    )
    {
        this.villanonoRepository = villanonoRepository;
        batchSize = elasticSearchSettings.Value.BatchSize;
    }

    public async Task BulkInsert<T>(Stream stream)
    {
        var records = new List<T>();

        using var streamReader = new StreamReader(stream);

        await foreach (var record in ReadCsvFile<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await villanonoRepository.BulkInsert(records);
                records.Clear();
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await villanonoRepository.BulkInsert(records);
        }
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

        while (await csv.ReadAsync())
        {
            yield return csv.GetRecord<T>();
        }
    }
}

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class CSVReader : ICSVReader
{
    public async IAsyncEnumerable<T> Read<T>(StreamReader stream)
        where T : VillanonoBaseModel
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
}

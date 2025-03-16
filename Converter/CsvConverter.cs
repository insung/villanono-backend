using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class VillanonoDateTimeConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(
        string? text,
        IReaderRow row,
        MemberMapData memberMapData
    )
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
            return null;
        else if (DateTime.TryParse(text, out DateTime date))
        {
            return date;
        }

        throw new TypeConverterException(this, memberMapData, text, row.Context);
    }
}

public class VillanonoStringConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(
        string? text,
        IReaderRow row,
        MemberMapData memberMapData
    )
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
            return null;
        else
            return text;
    }
}

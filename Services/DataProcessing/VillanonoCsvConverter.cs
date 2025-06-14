using System.Globalization;
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
        else if (DateTime.TryParseExact(text, "yy.MM.dd", null, DateTimeStyles.None, out date))
        {
            // 1900년대를 2000년대로 강제로 변경
            int fullYear = date.Year + (date.Year < 2000 ? 100 : 0);
            date = new DateTime(fullYear, date.Month, date.Day);
            return date;
        }

        throw new TypeConverterException(
            this,
            memberMapData,
            $"Invalid date format: {text}",
            row.Context
        );
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

public class VillanonoIntConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(
        string? text,
        IReaderRow row,
        MemberMapData memberMapData
    )
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
            return null;
        else if (int.TryParse(text, out int value))
        {
            return value;
        }

        throw new TypeConverterException(this, memberMapData, text, row.Context);
    }
}

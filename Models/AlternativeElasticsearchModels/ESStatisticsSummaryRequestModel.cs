using System.Text.Json;
using System.Text.Json.Serialization;

[JsonConverter(typeof(ESStatisticsSummaryRequestConverter))]
public class ESStatisticsSummaryRequest
{
    public VillanonoDataType DataType { get; set; }
    public int BeginDate { get; set; }
    public int EndDate { get; set; }
    public string Dong { get; set; }
    public string Gu { get; set; }
    public string Si { get; set; }
    public string GroupByKey { get; set; }
    public string GroupByValue { get; set; }
}

public class ESStatisticsSummaryRequestConverter : JsonConverter<ESStatisticsSummaryRequest>
{
    public override ESStatisticsSummaryRequest? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        ESStatisticsSummaryRequest value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        WriteBoolQuery(writer, value);
        WriteAggregations(writer, value);
        writer.WriteNumber("size", 0);
        writer.WriteEndObject();
    }

    private void WriteBoolQuery(Utf8JsonWriter writer, in ESStatisticsSummaryRequest value)
    {
        writer.WritePropertyName("query");
        writer.WriteStartObject();
        writer.WritePropertyName("bool");
        writer.WriteStartObject();
        writer.WritePropertyName("filter");
        WriteFilterQuery(writer, value);
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private void WriteFilterQuery(Utf8JsonWriter writer, in ESStatisticsSummaryRequest value)
    {
        writer.WriteStartArray();
        WriteTerm(writer, "dong", value.Dong);
        WriteTerm(writer, "gu", value.Gu);
        WriteTerm(writer, "si", value.Si);
        WriteTerm(writer, "dataType", value.DataType.ToString());
        WriteRange(writer, value.BeginDate, value.EndDate);
        writer.WriteEndArray();
    }

    private void WriteTerm(Utf8JsonWriter writer, string field, in string value)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("term");
        writer.WriteStartObject();
        writer.WriteString($"{field}.keyword", value);
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private void WriteTerms(Utf8JsonWriter writer, in string value)
    {
        writer.WritePropertyName("terms");
        writer.WriteStartObject();
        writer.WriteString("field", value);
        writer.WriteEndObject();
    }

    private void WriteRange(
        Utf8JsonWriter writer,
        in int beginDate,
        in int endDate,
        in string rangeColumnName = "contractDate"
    )
    {
        writer.WriteStartObject();
        writer.WritePropertyName("range");
        writer.WriteStartObject();
        writer.WritePropertyName(rangeColumnName);
        writer.WriteStartObject();
        writer.WriteString("gte", beginDate.ToString());
        writer.WriteString("lte", endDate.ToString());
        writer.WriteEndObject();
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private void WriteAggregations(Utf8JsonWriter writer, in ESStatisticsSummaryRequest value)
    {
        writer.WritePropertyName("aggs");
        writer.WriteStartObject();
        writer.WritePropertyName("key");
        writer.WriteStartObject();
        WriteTerms(writer, value.GroupByKey);
        WriteStatistics(writer, value);
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private void WriteStatistics(Utf8JsonWriter writer, in ESStatisticsSummaryRequest value)
    {
        writer.WritePropertyName("aggs");
        writer.WriteStartObject();
        WriteStatisticsItem(writer, "average", value.GroupByValue, "avg");
        WriteStatisticsItem(writer, "min", value.GroupByValue);
        WriteStatisticsItem(writer, "max", value.GroupByValue);
        WritePercentiles(writer, "percentiles", value.GroupByValue);
        writer.WriteEndObject();
    }

    private void WriteStatisticsItem(
        Utf8JsonWriter writer,
        string field,
        string value,
        string? functionName = null
    )
    {
        writer.WritePropertyName(field);
        writer.WriteStartObject();
        if (string.IsNullOrEmpty(functionName))
            writer.WritePropertyName(field);
        else
            writer.WritePropertyName(functionName);
        writer.WriteStartObject();
        writer.WriteString("field", value);
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private void WritePercentiles(Utf8JsonWriter writer, string field, string value)
    {
        writer.WritePropertyName(field);
        writer.WriteStartObject();
        writer.WritePropertyName("percentiles");
        writer.WriteStartObject();
        writer.WriteString("field", value);
        writer.WritePropertyName("percents");
        writer.WriteStartArray();
        writer.WriteNumberValue(25);
        writer.WriteNumberValue(50);
        writer.WriteNumberValue(75);
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.WriteEndObject();
    }
}

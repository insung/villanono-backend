using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VillanonoDataType
{
    BuySell,
    Rent,
}

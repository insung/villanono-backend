using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VillanonoDataType
{
    /// <summary>
    /// 매매 데이터
    /// </summary>
    BuySell,

    /// <summary>
    /// 임대 데이터
    /// </summary>
    Rent,
}

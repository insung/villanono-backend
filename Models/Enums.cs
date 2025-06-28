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

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AddressType
{
    /// <summary>
    /// 도로명 검색
    /// </summary>
    Road,

    /// <summary>
    /// 지번(지번주소) 검색
    /// </summary>
    Parcel,
}

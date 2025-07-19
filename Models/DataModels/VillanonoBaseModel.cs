public class VillanonoBaseModel
{
    /// <summary>
    /// 시
    /// </summary>
    public required string Si { get; set; }

    /// <summary>
    /// 구
    /// </summary>
    public required string Gu { get; set; }

    /// <summary>
    /// 동
    /// </summary>
    public required string Dong { get; set; }

    /// <summary>
    /// 번지
    /// </summary>
    public string? AddressNumber { get; set; }

    /// <summary>
    /// 본번
    /// </summary>
    public int MainNumber { get; set; }

    /// <summary>
    /// 부번
    /// </summary>
    public int SubNumber { get; set; }

    /// <summary>
    /// 건물명
    /// </summary>
    public string? BuildingName { get; set; }

    /// <summary>
    /// 전용면적
    /// </summary>
    public double ExclusiveArea { get; set; }

    /// <summary>
    /// 계약년월
    /// </summary>
    public int ContractYearMonth { get; set; }

    /// <summary>
    /// 계약일
    /// </summary>
    public int ContractDay { get; set; }

    /// <summary>
    /// 거래금액
    /// </summary>
    public double TransactionAmount { get; set; }

    /// <summary>
    /// 건축년도
    /// </summary>
    public int ConstructionYear { get; set; }

    /// <summary>
    /// 도로명
    /// </summary>
    public required string RoadName { get; set; }

    /// <summary>
    /// 층
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 계약일자
    /// </summary>
    public int ContractDate { get; set; }

    public virtual VillanonoDataType DataType { get; set; }
}

using CsvHelper.Configuration.Attributes;

public abstract class VillanonoBaseModel
{
    /// <summary>
    /// 시
    /// </summary>
    [Name("si")]
    public string Si { get; set; }

    /// <summary>
    /// 구
    /// </summary>
    [Name("gu")]
    public string Gu { get; set; }

    /// <summary>
    /// 동
    /// </summary>
    [Name("dong")]
    public string Dong { get; set; }

    /// <summary>
    /// 번지
    /// </summary>
    [Name("address_number")]
    public string AddressNumber { get; set; }

    /// <summary>
    /// 본번
    /// </summary>
    [Name("main_number")]
    public int MainNumber { get; set; }

    /// <summary>
    /// 부번
    /// </summary>
    [Name("sub_number")]
    public int SubNumber { get; set; }

    /// <summary>
    /// 건물명
    /// </summary>
    [Name("building_name")]
    public string BuildingName { get; set; }

    /// <summary>
    /// 전용면적
    /// </summary>
    [Name("exclusive_area")]
    public double ExclusiveArea { get; set; }

    /// <summary>
    /// 계약년월
    /// </summary>
    [Name("contract_year_month")]
    public int ContractYearMonth { get; set; }

    /// <summary>
    /// 계약일
    /// </summary>
    [Name("contract_day")]
    public int ContractDay { get; set; }

    /// <summary>
    /// 거래금액
    /// </summary>
    [Name("transaction_amount")]
    public double TransactionAmount { get; set; }

    /// <summary>
    /// 건축년도
    /// </summary>
    [Name("construction_year")]
    public int ConstructionYear { get; set; }

    /// <summary>
    /// 도로명
    /// </summary>
    [Name("road_name")]
    public string RoadName { get; set; }

    /// <summary>
    /// 층
    /// </summary>
    [Name("floor")]
    public int Floor { get; set; }

    /// <summary>
    /// 계약일자
    /// </summary>
    [Name("contract_date")]
    public int ContractDate { get; set; }

    public abstract VillanonoDataType DataType { get; }
}

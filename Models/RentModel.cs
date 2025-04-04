using CsvHelper.Configuration.Attributes;

public class RentModel : VillanonoBaseModel
{
    public override VillanonoDataType DataType => VillanonoDataType.Rent;

    /// <summary>
    /// 전월세구분
    /// </summary>
    [Name("lease_type")]
    public required string LeaseType { get; set; }

    /// <summary>
    /// 보증금(만원)
    /// </summary>
    [Name("deposit_amount")]
    public required double DepositAmount { get; set; }

    /// <summary>
    /// 월세금(만원)
    /// </summary>
    [Name("monthly_rent_amount")]
    public double MonthlyRentAmount { get; set; }

    /// <summary>
    /// 계약기간
    /// </summary>
    [Name("contract_period")]
    public int ContractPeriod { get; set; }

    /// <summary>
    /// 계약구분 (신규, 갱신, -)
    /// </summary>
    [Name("contract_type")]
    public string? ContractType { get; set; }

    /// <summary>
    /// 갱신요구권 사용 (사용, -)
    /// </summary>
    [Name("renewal_rights_used")]
    public string? RenewalRightsUsed { get; set; }

    /// <summary>
    /// 주택유형
    /// </summary>
    [Name("housing_type")]
    public string? HousingType { get; set; }
}

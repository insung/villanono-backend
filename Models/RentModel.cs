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
    public string? ContractPeriod { get; set; }

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

    /// <summary>
    /// 계약기간_시작
    /// </summary>
    [Name("contract_period_start")]
    public int? ContractPeriodStart { get; set; }

    /// <summary>
    /// 계약기간_종료
    /// </summary>
    [Name("contract_period_end")]
    public int? ContractPeriodEnd { get; set; }

    /// <summary>
    /// 종전계약 보증금(만원)
    /// </summary>
    [Name("previous_deposit_amount")]
    public double? PreviousDepositAmount { get; set; }

    /// <summary>
    /// 종전계약 월세(만원)
    /// </summary>
    [Name("previous_monthly_rent")]
    public double? PreviousMonthlyRent { get; set; }
}

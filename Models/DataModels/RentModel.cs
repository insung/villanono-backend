public class RentModel : VillanonoBaseModel
{
    public override VillanonoDataType DataType
    {
        get => VillanonoDataType.Rent;
        set { } // 자식 클래스에서는 이 값을 변경할 수 없도록 setter를 비워둡니다.
    }

    /// <summary>
    /// 전월세구분
    /// </summary>
    public required string LeaseType { get; set; }

    /// <summary>
    /// 보증금(만원)
    /// </summary>
    public required double DepositAmount { get; set; }

    /// <summary>
    /// 월세금(만원)
    /// </summary>
    public double MonthlyRentAmount { get; set; }

    /// <summary>
    /// 계약기간
    /// </summary>
    public string? ContractPeriod { get; set; }

    /// <summary>
    /// 계약구분 (신규, 갱신, -)
    /// </summary>
    public string? ContractType { get; set; }

    /// <summary>
    /// 갱신요구권 사용 (사용, -)
    /// </summary>
    public string? RenewalRightsUsed { get; set; }

    /// <summary>
    /// 주택유형
    /// </summary>
    public string? HousingType { get; set; }

    /// <summary>
    /// 계약기간_시작
    /// </summary>
    public int? ContractPeriodStart { get; set; }

    /// <summary>
    /// 계약기간_종료
    /// </summary>
    public int? ContractPeriodEnd { get; set; }

    /// <summary>
    /// 종전계약 보증금(만원)
    /// </summary>
    public double? PreviousDepositAmount { get; set; }

    /// <summary>
    /// 종전계약 월세(만원)
    /// </summary>
    public double? PreviousMonthlyRent { get; set; }
}

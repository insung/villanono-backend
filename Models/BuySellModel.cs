using CsvHelper.Configuration.Attributes;

public class BuySellModel
{
    [Name("si")]
    public string Si { get; set; } // "시"

    [Name("gu")]
    public string Gu { get; set; } // "구"

    [Name("dong")]
    public string Dong { get; set; } // "동"

    [Name("address_number")]
    public string AddressNumber { get; set; } // "번지"

    [Name("main_number")]
    public int MainNumber { get; set; } // "본번"

    [Name("sub_number")]
    public int SubNumber { get; set; } // "부번"

    [Name("building_name")]
    public string BuildingName { get; set; } // "건물명"

    [Name("exclusive_area")]
    public double ExclusiveArea { get; set; } // "전용면적"

    [Name("land_area")]
    public double LandArea { get; set; } // "대지권면적"

    [Name("contract_year_month")]
    public int ContractYearMonth { get; set; } // "계약년월"

    [Name("contract_day")]
    public int ContractDay { get; set; } // "계약일"

    [Name("transaction_amount")]
    public double TransactionAmount { get; set; } // "거래금액(

    [Name("floor")]
    public int Floor { get; set; } // "층"

    [Name("buyer")]
    public string? Buyer { get; set; } // "매수자"

    [Name("seller")]
    public string? Seller { get; set; } // "매도자"

    [Name("construction_year")]
    public int ConstructionYear { get; set; } // "건축년도"

    [Name("road_name")]
    public string RoadName { get; set; } // "도로명"

    [Name("release_reason_date")]
    public int? ReleaseReasonDate { get; set; } // "해제사유발생일"

    [Name("transaction_type")]
    public string? TransactionType { get; set; } // "거래유형"

    [Name("broker_location")]
    public string? BrokerLocation { get; set; } // "중개사소재지"

    [Name("registration_date")]
    public DateTime? RegistrationDate { get; set; } // "등기일자"

    [Name("contract_date")]
    public int ContractDate { get; set; } // "계약일자"
}

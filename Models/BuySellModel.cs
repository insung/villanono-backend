using CsvHelper.Configuration.Attributes;

public class BuySellModel
{
    [Name("si")]
    public string Si; // "시"

    [Name("gu")]
    public string Gu; // "구"

    [Name("dong")]
    public string Dong; // "동"

    [Name("address_number")]
    public string AddressNumber; // "번지"

    [Name("main_number")]
    public string MainNumber; // "본번"

    [Name("sub_number")]
    public string SubNumber; // "부번"

    [Name("building_name")]
    public string BuildingName; // "건물명"

    [Name("exclusive_area")]
    public string ExclusiveArea; // "전용면적"

    [Name("land_area")]
    public string LandArea; // "대지권면적"

    [Name("contract_year_month")]
    public string ContractYearMonth; // "계약년월"

    [Name("contract_day")]
    public string ContractDay; // "계약일"

    [Name("transaction_amount")]
    public string TransactionAmount; // "거래금액(

    [Name("floor")]
    public string Floor; // "층"

    [Name("buyer")]
    public string Buyer; // "매수자"

    [Name("seller")]
    public string Seller; // "매도자"

    [Name("construction_year")]
    public string ConstructionYear; // "건축년도"

    [Name("road_name")]
    public string RoadName; // "도로명"

    [Name("release_reason_date")]
    public string ReleaseReasonDate; // "해제사유발생일"

    [Name("transaction_type")]
    public string TransactionType; // "거래유형"

    [Name("broker_location")]
    public string BrokerLocation; // "중개사소재지"

    [Name("registration_date")]
    public string RegistrationDate; // "등기일자"

    [Name("contract_date")]
    public string ContractDate; // "계약일자"
}

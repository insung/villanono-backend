using CsvHelper.Configuration;

/// <summary>
/// VillanonoBaseModel에 대한 기본 매핑 규칙을 정의합니다.
/// 상속을 통해 자식 클래스에서 공통 매핑을 재사용할 수 있습니다.
/// </summary>
public class VillanonoBaseModelMap<T> : ClassMap<T>
    where T : VillanonoBaseModel
{
    public VillanonoBaseModelMap()
    {
        Map(m => m.Si).Name("si");
        Map(m => m.Gu).Name("gu");
        Map(m => m.Dong).Name("dong");
        Map(m => m.AddressNumber).Name("address_number");
        Map(m => m.MainNumber).Name("main_number");
        Map(m => m.SubNumber).Name("sub_number");
        Map(m => m.BuildingName).Name("building_name");
        Map(m => m.ExclusiveArea).Name("exclusive_area");
        Map(m => m.ContractYearMonth).Name("contract_year_month");
        Map(m => m.ContractDay).Name("contract_day");
        Map(m => m.TransactionAmount).Name("transaction_amount");
        Map(m => m.Floor).Name("floor");
        Map(m => m.ConstructionYear).Name("construction_year");
        Map(m => m.RoadName).Name("road_name");
        Map(m => m.ContractDate).Name("contract_date");
        Map(m => m.DataType).Ignore(); // DataType 속성은 CSV 파일에 없으므로 무시합니다.
    }
}

/// <summary>
/// BuySellModel에 대한 CSV 매핑 규칙을 정의합니다.
/// </summary>
public sealed class BuySellModelMap : VillanonoBaseModelMap<BuySellModel>
{
    public BuySellModelMap()
    {
        Map(m => m.LandShareArea).Name("land_share_area");
        Map(m => m.Buyer).Name("buyer");
        Map(m => m.Seller).Name("seller");
        Map(m => m.ReleaseReasonDate).Name("release_reason_date");
        Map(m => m.TransactionType).Name("transaction_type");
        Map(m => m.BrokerLocation).Name("broker_location");
        Map(m => m.RegistrationDate).Name("registration_date");
    }
}

/// <summary>
/// RentModel에 대한 CSV 매핑 규칙을 정의합니다.
/// </summary>
public sealed class RentModelMap : VillanonoBaseModelMap<RentModel>
{
    public RentModelMap()
    {
        Map(m => m.LeaseType).Name("lease_type");
        Map(m => m.DepositAmount).Name("deposit_amount");
        Map(m => m.MonthlyRentAmount).Name("monthly_rent_amount");
        Map(m => m.ContractPeriod).Name("contract_period");
        Map(m => m.ContractType).Name("contract_type");
        Map(m => m.RenewalRightsUsed).Name("renewal_rights_used");
        Map(m => m.HousingType).Name("housing_type");
        Map(m => m.ContractPeriodStart).Name("contract_period_start");
        Map(m => m.ContractPeriodEnd).Name("contract_period_end");
        Map(m => m.PreviousDepositAmount).Name("previous_deposit_amount");
        Map(m => m.PreviousMonthlyRent).Name("previous_monthly_rent");
    }
}

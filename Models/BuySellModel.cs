using CsvHelper.Configuration.Attributes;

public class BuySellModel : VillanonoBaseModel
{
    public override VillanonoDataType DataType => VillanonoDataType.BuySell;

    /// <summary>
    /// 대지권면적
    /// </summary>
    [Name("land_area")]
    public double LandArea { get; set; }

    /// <summary>
    /// 매수자
    /// </summary>
    [Name("buyer")]
    public string? Buyer { get; set; }

    /// <summary>
    /// 매도자
    /// </summary>
    [Name("seller")]
    public string? Seller { get; set; }

    /// <summary>
    /// 해제사유발생일
    /// </summary>
    [Name("release_reason_date")]
    public int? ReleaseReasonDate { get; set; }

    /// <summary>
    /// 거래유형
    /// </summary>
    [Name("transaction_type")]
    public string? TransactionType { get; set; }

    /// <summary>
    /// 중개사소재지
    /// </summary>
    [Name("broker_location")]
    public string? BrokerLocation { get; set; }

    /// <summary>
    /// 등기일자
    /// </summary>
    [Name("registration_date")]
    public DateTime? RegistrationDate { get; set; }
}

// public static class DataFrameHelper
// {
//     public static DataFrame CreateBuySellDataFrame(IReadOnlyCollection<BuySellModel> data)
//     {
//         if (data == null || !data.Any())
//         {
//             throw new ArgumentException("데이터가 비어 있습니다.", nameof(data));
//         }

//         // DataFrame 객체 생성
//         var dataFrame = new DataFrame();

//         // 열 추가
//         dataFrame.Columns.Add(new StringDataFrameColumn("Si", data.Select(x => x.Si)));
//         dataFrame.Columns.Add(new StringDataFrameColumn("Gu", data.Select(x => x.Gu)));
//         dataFrame.Columns.Add(new StringDataFrameColumn("Dong", data.Select(x => x.Dong)));
//         dataFrame.Columns.Add(
//             new StringDataFrameColumn("AddressNumber", data.Select(x => x.AddressNumber))
//         );
//         dataFrame.Columns.Add(
//             new Int32DataFrameColumn("MainNumber", data.Select(x => x.MainNumber))
//         );
//         dataFrame.Columns.Add(new Int32DataFrameColumn("SubNumber", data.Select(x => x.SubNumber)));
//         dataFrame.Columns.Add(
//             new StringDataFrameColumn("BuildingName", data.Select(x => x.BuildingName))
//         );
//         dataFrame.Columns.Add(
//             new DoubleDataFrameColumn("ExclusiveArea", data.Select(x => x.ExclusiveArea))
//         );
//         dataFrame.Columns.Add(new DoubleDataFrameColumn("LandArea", data.Select(x => x.LandArea)));
//         dataFrame.Columns.Add(
//             new Int32DataFrameColumn("ContractYearMonth", data.Select(x => x.ContractYearMonth))
//         );
//         dataFrame.Columns.Add(
//             new Int32DataFrameColumn("ContractDay", data.Select(x => x.ContractDay))
//         );
//         dataFrame.Columns.Add(
//             new DoubleDataFrameColumn("TransactionAmount", data.Select(x => x.TransactionAmount))
//         );
//         dataFrame.Columns.Add(new Int32DataFrameColumn("Floor", data.Select(x => x.Floor)));
//         dataFrame.Columns.Add(new StringDataFrameColumn("Buyer", data.Select(x => x.Buyer)));
//         dataFrame.Columns.Add(new StringDataFrameColumn("Seller", data.Select(x => x.Seller)));
//         dataFrame.Columns.Add(
//             new Int32DataFrameColumn("ConstructionYear", data.Select(x => x.ConstructionYear))
//         );
//         dataFrame.Columns.Add(new StringDataFrameColumn("RoadName", data.Select(x => x.RoadName)));
//         dataFrame.Columns.Add(
//             new Int32DataFrameColumn("ReleaseReasonDate", data.Select(x => x.ReleaseReasonDate))
//         );
//         dataFrame.Columns.Add(
//             new StringDataFrameColumn("TransactionType", data.Select(x => x.TransactionType))
//         );
//         dataFrame.Columns.Add(
//             new StringDataFrameColumn("BrokerLocation", data.Select(x => x.BrokerLocation))
//         );

//         // DateTime? 처리
//         var registrationDateColumn = new StringDataFrameColumn(
//             "RegistrationDate",
//             data.Select(x => x.RegistrationDate?.ToString("yyyy-MM-dd") ?? string.Empty)
//         );
//         dataFrame.Columns.Add(registrationDateColumn);

//         dataFrame.Columns.Add(
//             new Int32DataFrameColumn("ContractDate", data.Select(x => x.ContractDate))
//         );

//         return dataFrame;
//     }
// }

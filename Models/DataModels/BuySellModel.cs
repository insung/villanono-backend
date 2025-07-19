public class BuySellModel : VillanonoBaseModel
{
    public override VillanonoDataType DataType
    {
        get => VillanonoDataType.BuySell;
        set { } // 자식 클래스에서는 이 값을 변경할 수 없도록 setter를 비워둡니다.
    }

    /// <summary>
    /// 대지권면적
    /// </summary>
    public double LandShareArea { get; set; }

    /// <summary>
    /// 매수자
    /// </summary>
    public string? Buyer { get; set; }

    /// <summary>
    /// 매도자
    /// </summary>
    public string? Seller { get; set; }

    /// <summary>
    /// 해제사유발생일
    /// </summary>
    public int? ReleaseReasonDate { get; set; }

    /// <summary>
    /// 거래유형
    /// </summary>
    public string? TransactionType { get; set; }

    /// <summary>
    /// 중개사소재지
    /// </summary>
    public string? BrokerLocation { get; set; }

    /// <summary>
    /// 등기일자
    /// </summary>
    public DateTime? RegistrationDate { get; set; }
}
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

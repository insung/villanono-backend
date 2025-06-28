using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 주소 정보(도로명, 지번)를 포함한 모델
/// </summary>
public class AddressModel : RegionModel
{
    public required string AddressNumber { get; init; }
    public required string RoadName { get; init; }

    public AddressModel() { }

    [SetsRequiredMembers]
    public AddressModel(string si, string gu, string dong, string addressNumber, string roadName)
        : base(si, gu, dong)
    {
        AddressNumber = addressNumber;
        RoadName = roadName;
    }
}

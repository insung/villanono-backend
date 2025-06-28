using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 빌딩명을 포함한 주소 모델
/// </summary>
public class BuildingModel : GeocodeModel
{
    public required string? BuildingName { get; init; }

    public BuildingModel() { }

    [SetsRequiredMembers]
    public BuildingModel(
        AddressModel address,
        double? latitude,
        double? longitude,
        string? buildingName
    )
        : base(address, latitude, longitude)
    {
        BuildingName = buildingName;
    }
}

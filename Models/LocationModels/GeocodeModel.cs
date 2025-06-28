using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 경도, 위도를 포함한 모델
/// </summary>
public class GeocodeModel : AddressModel
{
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public GeocodeModel() { }

    [SetsRequiredMembers]
    public GeocodeModel(AddressModel address, double? latitude, double? longitude)
        : base(address.Si, address.Gu, address.Dong, address.AddressNumber, address.RoadName)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}

using System.Diagnostics.CodeAnalysis;

public class LocationModel
{
    public required string Si { get; init; }
    public required string Gu { get; init; }
    public required string Dong { get; init; }

    public LocationModel() { }

    [SetsRequiredMembers]
    public LocationModel(string si, string gu, string dong)
    {
        Si = si;
        Gu = gu;
        Dong = dong;
    }
}

public class AddressModel : LocationModel
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

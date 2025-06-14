public class LocationModel
{
    public required string Si { get; set; }
    public required string Gu { get; set; }
    public required string Dong { get; set; }
}

public class AddressModel : LocationModel
{
    public required string AddressNumber { get; set; }
    public required string RoadName { get; set; }
}

public class GeocodeModel : AddressModel
{
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }
}

using Refit;

public interface IVWorldRepository
{
    [Get("/req/address?service=address&request=getCoord&type=ROAD")]
    Task<VWorldGeocodeResponse> GetCoordinatesAsync([Query] string key, [Query] string address);
}

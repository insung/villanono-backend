using OpenSearch.Client;

public class GeocodeModelQueryStrategy : IAddressQueryStrategy<GeocodeModel>
{
    public CompositeAggregationSourcesDescriptor<GeocodeModel> ConfigureAggregationSources(
        CompositeAggregationSourcesDescriptor<GeocodeModel> sources
    )
    {
        // GeocodeModel은 좌표 정보까지 포함하여 그룹화합니다.
        sources
            .Terms("si", t => t.Field(f => f.Si.Suffix("keyword")))
            .Terms("gu", t => t.Field(f => f.Gu.Suffix("keyword")))
            .Terms("dong", t => t.Field(f => f.Dong.Suffix("keyword")))
            .Terms("roadName", t => t.Field(f => f.RoadName.Suffix("keyword")))
            // .Terms("addressNumber", t => t.Field(f => f.AddressNumber))
            .Terms("latitude", t => t.Field(f => f.Latitude)) // latitude 추가
            .Terms("longitude", t => t.Field(f => f.Longitude)); // longitude 추가
        return sources;
    }

    public GeocodeModel CreateModelFromBucket(IReadOnlyDictionary<string, object> bucketKey)
    {
        return new GeocodeModel
        {
            Si = bucketKey["si"]?.ToString() ?? "",
            Gu = bucketKey["gu"]?.ToString() ?? "",
            Dong = bucketKey["dong"]?.ToString() ?? "",
            RoadName = bucketKey["roadName"]?.ToString() ?? "",
            AddressNumber = "", // bucketKey["addressNumber"]?.ToString() ?? "",
            // 좌표 정보를 Key에서 직접 가져와 할당합니다.
            Latitude = (bucketKey["latitude"] as double?),
            Longitude = (bucketKey["longitude"] as double?),
        };
    }
}

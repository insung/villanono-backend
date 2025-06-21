using OpenSearch.Client;

public class AddressModelQueryStrategy : IAddressQueryStrategy<AddressModel>
{
    public CompositeAggregationSourcesDescriptor<AddressModel> ConfigureAggregationSources(
        CompositeAggregationSourcesDescriptor<AddressModel> sources
    )
    {
        sources
            .Terms("si", t => t.Field(f => f.Si.Suffix("keyword")))
            .Terms("gu", t => t.Field(f => f.Gu.Suffix("keyword")))
            .Terms("dong", t => t.Field(f => f.Dong.Suffix("keyword")))
            .Terms("roadName", t => t.Field(f => f.RoadName.Suffix("keyword")));
        // .Terms("addressNumber", t => t.Field(f => f.AddressNumber));
        return sources;
    }

    public AddressModel CreateModelFromBucket(IReadOnlyDictionary<string, object> bucketKey)
    {
        return new AddressModel(
            si: bucketKey["si"]?.ToString() ?? "",
            gu: bucketKey["gu"]?.ToString() ?? "",
            dong: bucketKey["dong"]?.ToString() ?? "",
            roadName: bucketKey["roadName"]?.ToString() ?? "",
            addressNumber: "" //bucketKey["addressNumber"]?.ToString() ?? ""
        );
    }
}

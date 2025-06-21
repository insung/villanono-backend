using OpenSearch.Client;

/// <summary>
/// 특정 타입 T에 대한 OpenSearch 쿼리 및 결과 생성을 위한 전략 인터페이스
/// </summary>
public interface IAddressQueryStrategy<T>
    where T : AddressModel
{
    /// <summary>
    /// Composite Aggregation의 Sources 부분을 구성합니다.
    /// </summary>
    CompositeAggregationSourcesDescriptor<T> ConfigureAggregationSources(
        CompositeAggregationSourcesDescriptor<T> sources
    );

    /// <summary>
    /// Aggregation Bucket의 Key로부터 T 타입의 객체를 생성합니다.
    /// </summary>
    T CreateModelFromBucket(IReadOnlyDictionary<string, object> bucketKey);
}

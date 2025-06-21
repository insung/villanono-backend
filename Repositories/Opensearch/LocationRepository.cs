using System.Collections.Concurrent;
using OpenSearch.Client;

public class LocationRepository : ILocationRepository
{
    readonly OpenSearchClient opensearchClient;

    public LocationRepository(OpenSearchClient elasticsearchClient)
    {
        this.opensearchClient = elasticsearchClient;
    }

    public async Task CreateIndex(string indexName)
    {
        var response = await opensearchClient.Indices.CreateAsync(indexName);
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Create Index failed"
        );
    }

    public async Task BulkInsertLocations<T>(List<T> records, string indexName = "si-gu-dong")
        where T : VillanonoBaseModel
    {
        var bulkOperations = new ConcurrentBag<IBulkOperation>();

        Parallel.ForEach(
            records,
            record =>
            {
                var id = $"{record.Si}-{record.Gu}-{record.Dong}";
                var locationRecord = new LocationModel(
                    si: record.Si,
                    gu: record.Gu,
                    dong: record.Dong
                );
                var bulkOperaion = new BulkCreateOperation<object>(locationRecord) { Id = id };
                bulkOperations.Add(bulkOperaion);
            }
        );

        var bulkRequest = new BulkRequest(indexName) { Operations = bulkOperations.ToList() };
        var response = await opensearchClient.BulkAsync(bulkRequest);
    }

    public async Task<IList<string>> GetAllDong(
        string Si,
        string Gu,
        string indexName = "si-gu-dong"
    )
    {
        var response = await opensearchClient.SearchAsync<LocationModel>(s =>
            s.Index(indexName)
                .Size(0) // 결과 문서는 필요하지 않으므로 Size 0
                .Query(q =>
                    q.Bool(b =>
                        b.Must(
                            mq => mq.Term(t => t.Field(f => f.Si.Suffix("keyword")).Value(Si)),
                            mq => mq.Term(t => t.Field(f => f.Gu.Suffix("keyword")).Value(Gu))
                        )
                    )
                )
                .Aggregations(a =>
                    a.Terms("dong", t => t.Field(f => f.Dong.Suffix("keyword")).Size(100))
                )
        );

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetAllSi Failed"
        );

        var result = response
            .Aggregations.Terms("dong")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<string>> GetAllGu(string si, string indexName = "si-gu-dong")
    {
        var response = await opensearchClient.SearchAsync<LocationModel>(s =>
            s.Index(indexName)
                .Size(0) // 결과 문서는 필요하지 않으므로 Size 0
                .Query(q => q.Term(t => t.Field(f => f.Si.Suffix("keyword")).Value(si)))
                .Aggregations(a =>
                    a.Terms("gu", t => t.Field(f => f.Gu.Suffix("keyword")).Size(100))
                )
        );

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetAllSi Failed"
        );

        var result = response
            .Aggregations.Terms("gu")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<string>> GetAllSi(string indexName = "si-gu-dong")
    {
        var response = await opensearchClient.SearchAsync<LocationModel>(s =>
            s.Index(indexName)
                .Size(0) // 결과 문서는 필요하지 않으므로 Size 0
                .Aggregations(a =>
                    a.Terms(
                        "si",
                        t =>
                            t.Field(f => f.Si.Suffix("keyword")) // 분석되지 않은 필드 사용
                                .Size(100) // 충분한 버킷 수
                    )
                )
        );

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetAllSi Failed"
        );

        var result = response
            .Aggregations.Terms("si")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<AddressModel>> GetDistinctAddress(
        string si,
        string indexName = "villanono-*"
    )
    {
        var allAddresses = new List<AddressModel>();
        CompositeKey? searchAfter = null;

        do
        {
            var response = await opensearchClient.SearchAsync<AddressModel>(s =>
                s.Index(indexName)
                    .Size(0)
                    .Query(q =>
                    {
                        QueryContainer query = q.Term(t => t.Field("si.keyword").Value(si.Trim()));
                        return query;
                    })
                    // 2. 필터링된 결과를 대상으로 집계를 수행합니다.
                    .Aggregations(aggs =>
                        aggs.Composite(
                            "distinct_addresses",
                            c =>
                                c
                                // 한 번에 가져올 고유 조합의 최대 개수를 지정합니다.
                                .Size(1000)
                                    .After(searchAfter)
                                    // =====> GROUP BY 할 필드들을 여기에 모두 정의합니다 <=====
                                    .Sources(sources =>
                                        sources
                                            .Terms("si", t => t.Field("si.keyword"))
                                            .Terms("gu", t => t.Field("gu.keyword"))
                                            .Terms("dong", t => t.Field("dong.keyword"))
                                            .Terms("roadName", t => t.Field("roadName.keyword"))
                                    // .Terms(
                                    //     "addressNumber",
                                    //     t => t.Field("addressNumber.keyword")
                                    // )
                                    )
                        )
                    )
            );

            OpensearchResponseHandler.CheckResponseFailed(
                response?.ApiCall?.HttpStatusCode,
                response?.ApiCall?.DebugInformation,
                "GetAllAddress failed"
            );

            var compositeAgg = response!.Aggregations.Composite("distinct_addresses");

            if (!compositeAgg.Buckets.Any())
                break;

            var distinctAddresses = compositeAgg.Buckets.Select(b => new AddressModel(
                si: b.Key["si"]?.ToString() ?? "",
                gu: b.Key["gu"]?.ToString() ?? "",
                dong: b.Key["dong"]?.ToString() ?? "",
                roadName: b.Key["roadName"]?.ToString() ?? "",
                addressNumber: "" // b.Key["addressNumber"]?.ToString() ?? "",
            ));

            allAddresses.AddRange(distinctAddresses);

            // 4. 다음 페이지를 위한 커서를 업데이트합니다.
            searchAfter = compositeAgg.AfterKey;
        } while (searchAfter != null && searchAfter.Count > 0); // 커서가 있는 동안 계속 반복

        return allAddresses;
    }

    public async Task<IList<T>> GetAddress<T>(
        IAddressQueryStrategy<T> strategy,
        string si,
        string gu = "",
        string roadName = "",
        string indexName = "geocode"
    )
        where T : AddressModel
    {
        var addressList = new List<T>();
        IReadOnlyCollection<object>? searchAfterValues = null;

        do
        {
            var response = await opensearchClient.SearchAsync<T>(s =>
                s.Index(indexName)
                    // 1. 실제 문서를 가져와야 하므로 Size를 0이 아닌 적절한 값으로 설정합니다.
                    .Size(1000)
                    // 2. 필터링 로직은 그대로 사용합니다.
                    .Query(q =>
                    {
                        QueryContainer query = q.Term(t => t.Field("si.keyword").Value(si.Trim()));

                        if (!string.IsNullOrWhiteSpace(gu))
                        {
                            query &= q.Term(t => t.Field("gu.keyword").Value(gu.Trim()));
                        }

                        if (!string.IsNullOrWhiteSpace(roadName))
                        {
                            query &= q.Prefix(p =>
                                p.Field("roadName.keyword").Value(roadName.Trim())
                            );
                        }

                        return query;
                    })
                    // 3. 페이지네이션을 위해 고유한 순서로 정렬합니다.
                    .Sort(so => so.Ascending("_id"))
                    // 4. 다음 페이지를 위한 커서를 설정합니다.
                    .SearchAfter(searchAfterValues)
            );

            // 응답 유효성 검사
            OpensearchResponseHandler.CheckResponseFailed(
                response?.ApiCall?.HttpStatusCode,
                response?.ApiCall?.DebugInformation,
                "GetAddress failed"
            );

            var documents = response!.Documents;
            if (!documents.Any())
            {
                break;
            }

            addressList.AddRange(documents);

            // 다음 페이지를 위해 커서를 업데이트합니다.
            searchAfterValues = response.Hits.LastOrDefault()?.Sorts;
        } while (searchAfterValues != null && searchAfterValues.Any());

        return addressList;
    }

    public async Task<bool> HasGeocode(
        string si,
        string gu,
        string roadName,
        string indexName = "geocode"
    )
    {
        var id = IdGenerator.GenerateDeterministicId(si, gu, roadName);
        var getResponse = await opensearchClient.GetAsync<GeocodeModel>(
            id,
            g => g.Index(indexName)
        );

        return getResponse?.Found ?? false;
    }

    public async Task UpsertGeocode(GeocodeModel geocodeModel, string indexName = "geocode")
    {
        var id = IdGenerator.GenerateDeterministicId(
            geocodeModel.Si,
            geocodeModel.Gu,
            geocodeModel.RoadName
        );
        await opensearchClient.IndexAsync(geocodeModel, i => i.Index(indexName).Id(id));
    }

    public async Task<int> GetGeocodeCount(string indexName = "geocode")
    {
        var response = await opensearchClient.CountAsync<GeocodeModel>(c => c.Index(indexName));
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetGeocodeCount failed"
        );
        return (int)response!.Count;
    }
}

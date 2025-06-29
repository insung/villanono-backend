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
                var locationRecord = new RegionModel(
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
        var response = await opensearchClient.SearchAsync<RegionModel>(s =>
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

        var result = response!
            .Aggregations.Terms("dong")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<string>> GetAllGu(string si, string indexName = "si-gu-dong")
    {
        var response = await opensearchClient.SearchAsync<RegionModel>(s =>
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

        var result = response!
            .Aggregations.Terms("gu")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<string>> GetAllSi(string indexName = "si-gu-dong")
    {
        var response = await opensearchClient.SearchAsync<RegionModel>(s =>
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

        var result = response!
            .Aggregations.Terms("si")
            .Buckets.Select(bucket => bucket.Key)
            .ToList();
        return result;
    }

    public async Task<IList<AddressModel>> GetDistinctAddress(
        string si,
        string gu = "",
        string dong = "",
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

                        // 2) gu 필터 (비어있지 않을 때만)
                        if (!string.IsNullOrWhiteSpace(gu))
                        {
                            query &= q.Term(t => t.Field("gu.keyword").Value(gu.Trim()));
                        }

                        // 3) dong 필터 (비어있지 않을 때만)
                        if (!string.IsNullOrWhiteSpace(dong))
                        {
                            query &= q.Term(t => t.Field("dong.keyword").Value(dong.Trim()));
                        }

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

    public async Task<long> GetDistinctAddressCardinalityCount(
        string si,
        string gu = "",
        string dong = "",
        string indexName = "villanono-*"
    )
    {
        var response = await opensearchClient.SearchAsync<GeocodeModel>(s =>
            s.Index(indexName)
                .Size(0)
                .Query(q =>
                {
                    // 기본 필터: si, gu, dong
                    var qc = q.Term(t => t.Field("si.keyword").Value(si.Trim()));
                    if (!string.IsNullOrWhiteSpace(gu))
                        qc &= q.Term(t => t.Field("gu.keyword").Value(gu.Trim()));
                    if (!string.IsNullOrWhiteSpace(dong))
                        qc &= q.Term(t => t.Field("dong.keyword").Value(dong.Trim()));
                    return qc;
                })
                .Aggregations(aggs =>
                    aggs.Cardinality(
                        // cardinality에 script를 써서 네 개 필드를 합쳐 고유 건수 계산
                        "distinct_address_count",
                        c =>
                            c.Script(scr =>
                                scr.Source(
                                    """
                                        doc['si.keyword'].value + '|' +
                                        doc['gu.keyword'].value + '|' +
                                        doc['dong.keyword'].value + '|' +
                                        doc['roadName.keyword'].value
                                    """
                                )
                            )
                    )
                )
        );

        OpensearchResponseHandler.CheckResponseFailed(
            response.ApiCall.HttpStatusCode,
            response.ApiCall.DebugInformation,
            "GetDistinctAddressCount failed"
        );

        // distinct_address_count 에 저장된 값을 꺼내서 리턴
        var card = response.Aggregations.Cardinality("distinct_address_count");
        return Convert.ToInt64(card?.Value ?? 0);
    }

    public async Task<IList<T>> SearchGeocode<T>(
        IAddressQueryStrategy<T> strategy,
        string si,
        string gu = "",
        string search = "",
        AddressType addressType = AddressType.Road,
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

                        if (!string.IsNullOrWhiteSpace(search))
                        {
                            if (addressType == AddressType.Parcel)
                            {
                                query &= q.Prefix(p =>
                                    p.Field("dong.keyword").Value(search.Trim())
                                );
                            }
                            else
                            {
                                query &= q.Prefix(p =>
                                    p.Field("roadName.keyword").Value(search.Trim())
                                );
                            }
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

    public async Task<long> GetTotalCount(string indexName = "geocode")
    {
        var response = await opensearchClient.CountAsync<AddressModel>(c => c.Index(indexName));
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetGeocodeCount failed"
        );
        return (int)response!.Count;
    }

    public async Task<T?> GetGeocode<T>(
        string si,
        string gu,
        string roadName,
        string indexName = "geocode"
    )
        where T : AddressModel
    {
        var id = IdGenerator.GenerateDeterministicId(si, gu, roadName);
        var getResponse = await opensearchClient.GetAsync<T>(id, g => g.Index(indexName));

        if (getResponse?.Found == true)
        {
            return getResponse.Source;
        }

        return null;
    }
}

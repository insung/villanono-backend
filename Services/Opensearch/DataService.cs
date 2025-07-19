using Microsoft.Extensions.Options;

/// <summary>
/// 빌라노노 데이터 서비스
/// </summary>
public class DataService : IDataService
{
    readonly IIndexManagementRepository indexManagementRepository;
    readonly IDataRepository dataRepository;
    readonly ILocationRepository locationRepository;
    readonly IVillanonoCsvReader villanonoCsvReader;
    readonly int batchSize;

    public DataService(
        IIndexManagementRepository indexManagementRepository,
        IDataRepository dataRepository,
        ILocationRepository locationRepository,
        IVillanonoCsvReader villanonoCsvReader,
        IOptions<OpensearchSettingsModel> elasticSearchSettings
    )
    {
        this.indexManagementRepository = indexManagementRepository;
        this.dataRepository = dataRepository;
        this.locationRepository = locationRepository;
        this.villanonoCsvReader = villanonoCsvReader;
        batchSize = elasticSearchSettings.Value.BatchSize;
    }

    public async Task<int> BulkInsertData<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel
    {
        var totalRowAffected = 0;
        var records = new List<T>();

        using var streamReader = new StreamReader(stream);

        if (!await indexManagementRepository.HasIndex(indexName))
            await indexManagementRepository.CreateDataIndex<T>(indexName);

        await foreach (var record in villanonoCsvReader.Read<T>(streamReader))
        {
            records.Add(record);

            if (records.Count >= batchSize)
            {
                await dataRepository.BulkInsertData(records, indexName);
                await locationRepository.BulkInsertLocations(records);
                records.Clear();
                totalRowAffected += batchSize;
            }
        }

        // 마지막에 남은 데이터 전송
        if (records.Count > 0)
        {
            await dataRepository.BulkInsertData(records, indexName);
            await locationRepository.BulkInsertLocations(records);
            totalRowAffected += records.Count;
        }

        return totalRowAffected;
    }

    public async Task<IReadOnlyCollection<VillanonoBaseWithGeocodeModel>> Search(
        HashSet<VillanonoDataType> dataTypes,
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        DateOnly? beginContractDate = null,
        DateOnly? endContractDate = null,
        double? beginTransactionAmount = null,
        double? endTransactionAmount = null,
        int? beginConstructYear = null,
        int? endConstructYear = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null
    )
    {
        // 1. dataRepository에서 필터에 맞는 데이터를 VillanonoBaseWithGeocodeModel로 직접 가져옵니다.
        //    이렇게 하면 나중에 속성을 복사할 필요가 없습니다.
        var searchResult = await dataRepository.SearchBySiGuDong<VillanonoBaseWithGeocodeModel>(
            dataTypes,
            si,
            gu,
            dong,
            beginContractDate,
            endContractDate,
            beginTransactionAmount,
            endTransactionAmount,
            beginConstructYear,
            endConstructYear,
            beginExclusiveArea,
            endExclusiveArea
        );

        if (!searchResult.Any())
        {
            return Array.Empty<VillanonoBaseWithGeocodeModel>();
        }

        // 2. 가져온 데이터에서 고유한 주소 목록을 추출합니다.
        var distinctAddresses = searchResult
            .DistinctBy(r => (r.Si, r.Gu, r.RoadName))
            .Select(r => new AddressModel(
                si: r.Si,
                gu: r.Gu,
                dong: r.Dong,
                addressNumber: r.AddressNumber ?? "",
                roadName: r.RoadName
            ))
            .ToList();

        // 3. 고유 주소에 대한 Geocode 정보를 Bulk로 가져옵니다.
        var geocodeList = await locationRepository.GetGeocodeList(distinctAddresses);
        var geocodeMap = geocodeList.ToDictionary(g => (g.Si, g.Gu, g.RoadName));

        // 4. 기존 검색 결과 객체에 Geocode 정보를 직접 업데이트합니다.
        //    새 객체를 생성하고 모든 속성을 복사하는 대신, 필요한 속성만 설정하여 성능을 개선합니다.
        foreach (var data in searchResult)
        {
            if (geocodeMap.TryGetValue((data.Si, data.Gu, data.RoadName), out var geocode))
            {
                data.Latitude = geocode.Latitude;
                data.Longitude = geocode.Longitude;
            }
        }

        return searchResult;
    }
}

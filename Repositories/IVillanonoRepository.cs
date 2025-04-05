public interface IVillanonoRepository
{
    ValueTask Ping();
    ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel;

    ValueTask CreateLocationsIndex();

    /// <summary>
    /// 빌라노노의 매매데이터, 임대데이터를 Bulk 로 삽입하는 메서드. 기존 데이터가 있다면 덮어쓰기 됨.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task BulkInsertData<T>(List<T> records, string? indexName = null)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 빌라노노의 시, 구, 동에 대한 정보를 Bulk 로 삽입하는 메서드. 중복된 것은 Skip 됨.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task BulkInsertLocations<T>(List<T> records)
        where T : VillanonoBaseModel;

    ValueTask<bool> HasIndex(string indexName);
    ValueTask DeleteIndex(string indexName);
    Task<IReadOnlyCollection<T>> GetData<T>(
        VillanonoDataType dataType,
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel;
    Task<StatisticalSummary> GetStatisticsSummary(
        VillanonoDataType dataType,
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    );
}

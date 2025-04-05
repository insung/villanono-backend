public interface IVillanonoRepository
{
    ValueTask Ping();
    ValueTask CreateIndex<T>(string indexName)
        where T : VillanonoBaseModel;
    Task BulkInsert<T>(List<T> records, string? indexName = null)
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

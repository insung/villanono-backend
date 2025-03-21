public interface IVillanonoRepository
{
    ValueTask Ping();
    ValueTask CreateIndex(string indexName);
    Task BulkInsert<T>(List<T> records, string? indexName = null);
    ValueTask<bool> HasIndex(string indexName);
    ValueTask DeleteIndex(string indexName);
    Task<IReadOnlyCollection<T>> GetData<T>(
        string indexName,
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    );
}

public interface IVillanonoRepository
{
    ValueTask Ping();
    ValueTask CreateIndex(string indexName);
    Task BulkInsert<T>(List<T> records, string? indexName = null);
    ValueTask<bool> HasIndex(string indexName);
    ValueTask DeleteIndex(string indexName);
}

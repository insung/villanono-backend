public interface IVillanonoLoadService
{
    ValueTask RepositoryHealthCheck();
    ValueTask CreateIndex(string indexName);
    Task<int> BulkInsert<T>(Stream stream, string indexName);
    ValueTask DeleteIndex(string indexName);
}

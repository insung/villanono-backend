public interface IRepositoryService
{
    ValueTask HealthCheck();
    ValueTask CreateIndex(string indexName);
    Task<int> BulkInsert<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel;
    ValueTask DeleteIndex(string indexName);
}

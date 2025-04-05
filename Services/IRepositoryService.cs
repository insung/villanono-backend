public interface IRepositoryService
{
    ValueTask HealthCheck();
    ValueTask CreateIndex<T>(string indexName)
        where T : VillanonoBaseModel;
    Task<int> BulkInsert<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel;
    ValueTask DeleteIndex(string indexName);
}

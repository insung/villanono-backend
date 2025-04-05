public interface IRepositoryService
{
    ValueTask HealthCheck();
    ValueTask CreateIndex<T>(string indexName)
        where T : VillanonoBaseModel;
    Task<int> BulkInsertData<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel;
    Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel;
    ValueTask DeleteIndex(string indexName);
}

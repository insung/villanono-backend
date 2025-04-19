public interface IIndexManagementService
{
    ValueTask HealthCheck();
    ValueTask CreateIndex<T>(string indexName)
        where T : VillanonoBaseModel;
    ValueTask DeleteIndex(string indexName);
}

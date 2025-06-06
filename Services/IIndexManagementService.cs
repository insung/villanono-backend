public interface IIndexManagementService
{
    ValueTask HealthCheck();
    ValueTask<string> CreateIndex(string indexName);
    ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel;
    ValueTask DeleteIndex(string indexName);
    ValueTask ReIndex(string sourceIndexName, string targetIndexName);
}

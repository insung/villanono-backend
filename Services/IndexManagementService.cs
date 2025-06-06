public class IndexManagementService : IIndexManagementService
{
    readonly IVillanonoRepository villanonoRepository;

    public IndexManagementService(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
    }

    public async ValueTask HealthCheck()
    {
        await villanonoRepository.Ping();
    }

    public async ValueTask<string> CreateIndex(string indexName)
    {
        if (!await villanonoRepository.HasIndex(indexName))
        {
            return await villanonoRepository.CreateIndex(indexName);
        }
        return indexName;
    }

    public async ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel
    {
        if (!await villanonoRepository.HasIndex(indexName))
            await villanonoRepository.CreateIndex(indexName);
    }

    public async ValueTask DeleteIndex(string indexName)
    {
        await villanonoRepository.DeleteIndex(indexName);
    }

    public async ValueTask ReIndex(string sourceIndexName, string targetIndexName)
    {
        await villanonoRepository.ReIndex(sourceIndexName, targetIndexName);
    }
}

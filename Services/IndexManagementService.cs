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

    public async ValueTask CreateIndex<T>(string indexName)
        where T : VillanonoBaseModel
    {
        if (!await villanonoRepository.HasIndex(indexName))
            await villanonoRepository.CreateDataIndex<T>(indexName);
    }

    public async ValueTask DeleteIndex(string indexName)
    {
        await villanonoRepository.DeleteIndex(indexName);
    }
}

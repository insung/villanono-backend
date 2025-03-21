public class BuySellDataService : IVillanonoDataService
{
    readonly IVillanonoRepository villanonoRepository;

    public BuySellDataService(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
    }

    public async Task<IReadOnlyCollection<T>> GetData<T>(
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        return await villanonoRepository.GetData<T>(
            "villanono-*",
            beginDate,
            endDate,
            dong,
            gu,
            si
        );
    }
}

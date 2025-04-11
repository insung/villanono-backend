/// <summary>
/// 빌라노노 데이터 서비스
/// </summary>
public class DataService : IDataService
{
    readonly IVillanonoRepository villanonoRepository;

    public DataService(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
    }

    public async Task<IReadOnlyCollection<VillanonoBaseModel>> GetData(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        if (dataType == VillanonoDataType.BuySell)
        {
            return await villanonoRepository.GetData<BuySellModel>(
                dataType,
                beginDate,
                endDate,
                dong,
                gu,
                si
            );
        }
        else if (dataType == VillanonoDataType.Rent)
        {
            return await villanonoRepository.GetData<RentModel>(
                dataType,
                beginDate,
                endDate,
                dong,
                gu,
                si
            );
        }
        else
        {
            throw new ArgumentException("Invalid dataType");
        }
    }

    public async Task<StatisticalSummary> GetStatisticsSummary(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        return await villanonoRepository.GetStatisticsSummary(
            dataType,
            beginDate,
            endDate,
            dong,
            gu,
            si
        );
    }
}

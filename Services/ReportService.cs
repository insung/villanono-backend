public class ReportService : IReportService
{
    readonly IVillanonoRepository villanonoRepository;

    public ReportService(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
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

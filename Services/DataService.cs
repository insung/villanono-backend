public class DataService : IDataService
{
    readonly IVillanonoRepository villanonoRepository;

    public DataService(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
    }

    public async Task<IReadOnlyCollection<VillanonoBaseModel>> GetData(
        VillanonoDataType dataType,
        int beginDate,
        int endDate,
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
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        var repositoryResponse = await villanonoRepository.GetStatisticsSummary(
            dataType,
            beginDate,
            endDate,
            dong,
            gu,
            si
        );

        return new StatisticalSummary(repositoryResponse);

        // if (dataFrame == null)
        //     return null;

        // var totalCount = dataFrame.Rows.Count;
        // var totalMean = dataFrame["TransactionAmount"].Mean();

        // // 공백 제거 및 데이터 타입 통일
        // GroupBy groupBy = dataFrame.GroupBy("ContractYearMonth");
        // // var uniqueYearMonth = dataFrame["ContractYearMonth"]
        // //     .Cast<int>()
        // //     .Distinct()
        // //     .OrderBy(x => x)
        // //     .ToList();

        // // var yearColumn = dataFrame["ContractYearMonth"].Uni
        // // var totalCount = groupBy.Count();
        // // var groupByRowsMean = groupBy.Mean("TransactionAmount");

        // var groupByRows = new List<KeyValuePair<long, double>>();

        // // foreach (var (key, value) in columnContractYearMonth.Zip(columnTransactionAmount))
        // // {
        // //     groupByRows.Add(new KeyValuePair<long, double>(key.Value, value.Value));
        // // }

        // // for (int i = 0; i < uniqueYearMonth.Count; i++)
        // // {
        // //     var yearMonth = uniqueYearMonth[i];
        // //     var meanValue = Convert.ToDouble(groupByRowsMean);
        // // }

        // // uniqueYearMonth.Zip(groupByRowsMean.Columns, (year, mean) => new KeyValuePair<int, double>(year, mean))

        // return new VillanonoStatistics
        // {
        //     CountOfRows = totalCount,
        //     AverageOfTransactionAmount = totalMean,
        //     groupByRowsMean = groupByRows,
        // };
    }
}

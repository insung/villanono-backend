public interface IDataService
{
    Task<int> BulkInsertData<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel;

    Task<IReadOnlyCollection<VillanonoBaseWithGeocodeModel>> Search(
        HashSet<VillanonoDataType> dataTypes,
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        DateOnly? beginContractDate = null,
        DateOnly? endContractDate = null,
        double? beginTransactionAmount = null,
        double? endTransactionAmount = null,
        int? beginConstructYear = null,
        int? endConstructYear = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null
    );
}

public interface IDataService
{
    /// <summary>
    /// Raw Data 가져오는 메서드
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="beginDate"></param>
    /// <param name="endDate"></param>
    /// <param name="dong"></param>
    /// <param name="gu"></param>
    /// <param name="si"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<VillanonoBaseModel>> GetData(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    );

    Task<int> BulkInsertData<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel;
}

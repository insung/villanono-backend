public interface IDataRepository
{
    /// <summary>
    /// 빌라노노의 매매데이터, 임대데이터를 Bulk 로 삽입하는 메서드. 기존 데이터가 있다면 덮어쓰기 됨.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task BulkInsertData<T>(List<T> records, string indexName)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 데이터를 가져오는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataType"></param>
    /// <param name="beginDate"></param>
    /// <param name="endDate"></param>
    /// <param name="dong"></param>
    /// <param name="gu"></param>
    /// <param name="si"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetData<T>(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel;
}

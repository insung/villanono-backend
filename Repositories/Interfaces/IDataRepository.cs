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
    /// 시, 구, 동을 기준으로 데이터를 가져오는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTypes"></param>
    /// <param name="si"></param>
    /// <param name="gu"></param>
    /// <param name="dong"></param>
    /// <param name="beginContractDate"></param>
    /// <param name="endContractDate"></param>
    /// <param name="beginTransactionAmount"></param>
    /// <param name="endTransactionAmount"></param>
    /// <param name="constructYear"></param>
    /// <param name="exclusiveArea"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> SearchBySiGuDong<T>(
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
        double? endExclusiveArea = null,
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel;

    /// <summary>
    /// 도로명, 주소를 기준으로 데이터를 검색하는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="roadName"></param>
    /// <param name="buildingName"></param>
    /// <param name="contractDate"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> SearchByRoadName<T>(
        string roadName,
        string buildingName,
        int? contractDate,
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel;
}

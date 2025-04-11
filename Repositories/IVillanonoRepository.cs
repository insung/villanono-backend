/// <summary>
/// Repository 인터페이스
/// </summary>
public interface IVillanonoRepository
{
    /// <summary>
    /// Ping 을 날려 Repository 가 살아있는지 확인
    /// </summary>
    /// <returns></returns>
    ValueTask Ping();

    #region IndexManagement
    /// <summary>
    /// 빌라노노 Data 인덱스를 만드는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 시, 구, 동 위치 정보 인덱스를 만드는 메서드
    /// </summary>
    /// <returns></returns>
    ValueTask CreateLocationsIndex();

    /// <summary>
    /// 인덱스가 있는지 확인하는 메서드
    /// </summary>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask<bool> HasIndex(string indexName);

    /// <summary>
    /// 인덱스를 삭제하는 메서드
    /// </summary>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask DeleteIndex(string indexName);
    #endregion

    #region Insert/BulkInsert
    /// <summary>
    /// 빌라노노의 매매데이터, 임대데이터를 Bulk 로 삽입하는 메서드. 기존 데이터가 있다면 덮어쓰기 됨.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task BulkInsertData<T>(List<T> records, string? indexName = null)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 빌라노노의 시, 구, 동에 대한 정보를 Bulk 로 삽입하는 메서드. 중복된 것은 Skip 됨.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task BulkInsertLocations<T>(List<T> records)
        where T : VillanonoBaseModel;
    #endregion

    #region Read Operations
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

    /// <summary>
    /// 통계 데이터를 가져오는 메서드
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="beginDate"></param>
    /// <param name="endDate"></param>
    /// <param name="dong"></param>
    /// <param name="gu"></param>
    /// <param name="si"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<StatisticalSummary> GetStatisticsSummary(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    );
    #endregion
}

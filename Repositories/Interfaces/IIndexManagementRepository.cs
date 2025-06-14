public interface IIndexManagementRepository
{
    /// <summary>
    /// Ping 을 날려 Repository 가 살아있는지 확인
    /// </summary>
    /// <returns></returns>
    ValueTask Ping();

    /// <summary>
    /// 인덱스 생성 메서드
    /// </summary>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask<string> CreateIndex(string indexName);

    /// <summary>
    /// 빌라노노 Data 인덱스를 만드는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel;

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

    /// <summary>
    /// 인덱스를 재구성하는 메서드
    /// </summary>
    /// <param name="sourceIndexName"></param>
    /// <param name="targetIndexName"></param>
    /// <returns></returns>
    ValueTask ReIndex(string sourceIndexName, string targetIndexName);
}

public interface ILocationService
{
    /// <summary>
    /// 빌라노노 위치정보를 Bulk Insert하는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 지오코드 정보를 인덱스에 삽입하거나 업데이트하는 메서드
    /// </summary>
    /// <param name="addressModels"></param>
    /// <param name="vWorldAPIRequestQuota"></param>
    /// <returns></returns>
    Task BulkInsertGeocode(IList<AddressModel> addressModels, int vWorldAPIRequestQuota = 1000);
}

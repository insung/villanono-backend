public interface ILocationService
{
    /// <summary>
    /// 빌라노노 시, 구, 동 정보를 Bulk Insert하는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task BulkInsertRegions<T>(Stream stream)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 도로명, 지번 주소 정보를 인덱스에 삽입하거나 업데이트하는 메서드
    /// </summary>
    /// <param name="addressModels"></param>
    /// <param name="vWorldAPIRequestQuota"></param>
    /// <returns></returns>
    Task BulkInsertAddress(IList<AddressModel> addressModels, int vWorldAPIRequestQuota = 1000);

    /// <summary>
    /// AddressModel 을 가져오고 위도, 경도 정보를 VWorld API를 통해 GeocodeModel 을 가져오는 메서드
    /// </summary>
    /// <param name="si">시 이름</param>
    /// <param name="gu">구 이름</param>
    /// <param name="roadName">도로명</param>
    /// <returns>지오코드 모델</returns>
    Task<GeocodeModel?> GetGeocodeWithVWorld(string si, string gu, string roadName);
}

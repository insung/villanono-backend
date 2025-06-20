public interface ILocationService
{
    /// <summary>
    /// "시" 목록을 가져오는 메서드
    /// </summary>
    /// <returns></returns>
    Task<IList<string>> GetAllSi();

    /// <summary>
    /// 선택한 "시"에 대한 "구" 목록을 가져오는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <returns></returns>
    Task<IList<string>> GetAllGu(string Si);

    /// <summary>
    /// 선택한 "시"와 "구"에 대한 "동" 목록을 가져오는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <returns></returns>
    Task<IList<string>> GetAllDong(string Si, string Gu);

    /// <summary>
    /// 빌라노노 위치정보를 Bulk Insert하는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel;

    /// <summary>
    /// 전달받은 시, 구, 도로명에 대한 모든 주소 정보를 반환하는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <param name="roadName"></param>
    /// <returns></returns>
    Task<IList<AddressModel>> GetAddress(string Si, string Gu = "", string roadName = "");

    /// <summary>
    /// 지오코드 정보를 인덱스에 삽입하거나 업데이트하는 메서드
    /// </summary>
    /// <param name="addressModels"></param>
    /// <returns></returns>
    Task BulkInsertGeocode(IList<AddressModel> addressModels);
}

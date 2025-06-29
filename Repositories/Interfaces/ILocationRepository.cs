public interface ILocationRepository
{
    /// <summary>
    /// Location 정보를 담을 수 있는 인덱스를 생성하는 메서드
    /// </summary>
    /// <returns></returns>
    Task CreateIndex(string indexName);

    /// <summary>
    /// 빌라노노의 시, 구, 동에 대한 정보를 Bulk 로 삽입하는 메서드. 중복된 것은 Skip 됨.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="records"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task BulkInsertLocations<T>(List<T> records, string indexName = "si-gu-dong")
        where T : VillanonoBaseModel;

    /// <summary>
    /// 대한민국의 모든 시에 대한 정보를 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    Task<IList<string>> GetAllSi(string indexName = "si-gu-dong");

    /// <summary>
    /// 전달받은 시에 대한 모든 구에 대한 정보를 반환하는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IList<string>> GetAllGu(string Si, string indexName = "si-gu-dong");

    /// <summary>
    /// 전달받은 시, 구에 대한 모든 동에 대한 정보를 반환하는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IList<string>> GetAllDong(string Si, string Gu, string indexName = "si-gu-dong");

    /// <summary>
    /// 빌라노노의 시, 구, 도로명에 대한 유니크한 주소 정보를 반환하는 메서드
    /// </summary>
    /// <param name="si"></param>
    /// <param name="gu"></param>
    /// <param name="dong"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IList<AddressModel>> GetDistinctAddress(
        string si,
        string gu = "",
        string dong = "",
        string indexName = "villanono-*"
    );

    /// <summary>
    /// 빌라노노의 시, 구, 도로명에 대한 유니크한 주소 카드리니티 개수를 반환하는 메서드
    /// </summary>
    /// <param name="si"></param>
    /// <param name="gu"></param>
    /// <param name="dong"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<long> GetDistinctAddressCardinalityCount(
        string si,
        string gu = "",
        string dong = "",
        string indexName = "villanono-*"
    );

    /// <summary>
    /// 전달받은 시, 구, 도로명에 대한 모든 주소 정보를 반환하는 메서드
    /// </summary>
    /// <param name="queryStrategy"></param>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <param name="search"></param>
    /// <param name="addressType"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IList<T>> SearchGeocode<T>(
        IAddressQueryStrategy<T> queryStrategy,
        string Si,
        string Gu = "",
        string search = "",
        AddressType addressType = AddressType.Road,
        string indexName = "geocode"
    )
        where T : AddressModel;

    /// <summary>
    /// 지오코드 정보가 있는지 확인하는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <param name="roadName"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<bool> HasGeocode(string Si, string Gu, string roadName, string indexName = "geocode");

    /// <summary>
    /// 지오코드 정보를 인덱스에 삽입하거나 업데이트하는 메서드
    /// </summary>
    /// <param name="geocodeModel"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task UpsertGeocode(GeocodeModel geocodeModel, string indexName = "geocode");

    /// <summary>
    /// 지오코드 인덱스의 총 개수를 반환하는 메서드
    /// </summary>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<long> GetTotalCount(string indexName = "geocode");

    /// <summary>
    /// 지오코드 정보를 가져오는 메서드
    /// </summary>
    /// <param name="si"></param>
    /// <param name="gu"></param>
    /// <param name="roadName"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<T?> GetGeocode<T>(string si, string gu, string roadName, string indexName = "geocode")
        where T : AddressModel;
}

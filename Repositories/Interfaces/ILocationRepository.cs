public interface ILocationRepository
{
    /// <summary>
    /// Location 정보를 담을 수 있는 인덱스를 생성하는 메서드
    /// </summary>
    /// <returns></returns>
    ValueTask CreateIndex(string indexName);

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
    /// 전달받은 시, 구, 도로명에 대한 모든 주소 정보를 반환하는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <param name="roadName"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<IList<AddressModel>> GetAddress(
        string Si,
        string Gu = "",
        string roadName = "",
        string indexName = "villanono-*"
    );

    /// <summary>
    /// 지오코드 정보가 있는지 확인하는 메서드
    /// </summary>
    /// <param name="Si"></param>
    /// <param name="Gu"></param>
    /// <param name="roadName"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask<bool> HasGeocode(string Si, string Gu, string roadName, string indexName = "geocode");

    /// <summary>
    /// 지오코드 정보를 인덱스에 삽입하거나 업데이트하는 메서드
    /// </summary>
    /// <param name="geocodeModel"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    ValueTask UpsertGeocode(GeocodeModel geocodeModel, string indexName = "geocode");
}

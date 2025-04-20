public interface ILocationService
{
    Task<IList<string>> GetAllSi();
    Task<IList<string>> GetAllGu(string Si);
    Task<IList<string>> GetAllDong(string Si, string Gu);
    Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel;
}

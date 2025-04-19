public interface ILocationService
{
    Task BulkInsertLocations<T>(Stream stream)
        where T : VillanonoBaseModel;
}

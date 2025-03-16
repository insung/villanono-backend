public interface IVillanonoLoadService
{
    Task BulkInsert<T>(Stream stream);
}

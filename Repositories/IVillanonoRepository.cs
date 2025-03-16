public interface IVillanonoRepository
{
    ValueTask Ping();

    ValueTask CreateDefaultDatabase();

    Task BulkInsert<T>(List<T> records, string? indexName = null);
}

public interface IDataService
{
    Task<int> BulkInsertData<T>(Stream stream, string indexName)
        where T : VillanonoBaseModel;
}

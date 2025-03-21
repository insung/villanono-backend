public interface IDataService
{
    Task<IReadOnlyCollection<T>> GetData<T>(
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    );
}

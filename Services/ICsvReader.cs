public interface ICSVReader
{
    IAsyncEnumerable<T> Read<T>(StreamReader stream)
        where T : VillanonoBaseModel;
}

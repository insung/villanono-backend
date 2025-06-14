public interface IVillanonoCsvReader
{
    IAsyncEnumerable<T> Read<T>(StreamReader stream)
        where T : VillanonoBaseModel;
}

interface IVillanonoRepository
{
    ValueTask Ping();

    ValueTask CreateDefaultDatabase();
}

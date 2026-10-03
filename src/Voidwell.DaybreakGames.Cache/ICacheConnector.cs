using StackExchange.Redis;

namespace Voidwell.DaybreakGames.Cache;

public interface ICacheConnector
{
    IDatabase? Database { get; }
    Task<IDatabaseAsync?> ConnectAsync(CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Voidwell.DaybreakGames.Cache;

public class CacheConnector : ICacheConnector, IDisposable
{
    private readonly IOptions<CacheOptions> _options;
    private ConnectionMultiplexer? _redis;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public IDatabase? Database { get; private set; }

    public CacheConnector(IOptions<CacheOptions> options)
    {
        _options = options;

        Task.Run(() => ConnectAsync());
    }

    public async Task<IDatabaseAsync?> ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Value.RedisConfiguration))
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (_redis != null)
            return Database;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_redis == null)
                _redis = await ConnectionMultiplexer.ConnectAsync(_options.Value.RedisConfiguration);

            Database = _redis.GetDatabase();
        }
        finally
        {
            _connectionLock.Release();
        }

        return Database;
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }
}

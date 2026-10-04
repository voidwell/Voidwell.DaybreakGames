using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Voidwell.DaybreakGames.Cache;

/// <summary>
/// Stores lists as Redis sets. Failures are swallowed so that a Redis outage never breaks callers;
/// reads report a miss (<c>null</c>) instead.
/// </summary>
public sealed class RedisListStore(IOptions<CacheOptions> options) : IListStore, IDisposable
{
    private readonly CacheOptions _options = options.Value;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private ConnectionMultiplexer? _redis;

    public async Task AddAsync(string key, string item)
    {
        try
        {
            var db = await GetDatabaseAsync();
            await db.SetAddAsync(FormatKey(key), item);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task RemoveAsync(string key, string item)
    {
        try
        {
            var db = await GetDatabaseAsync();
            await db.SetRemoveAsync(FormatKey(key), item);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task ClearAsync(string key)
    {
        try
        {
            var db = await GetDatabaseAsync();
            await db.KeyDeleteAsync(FormatKey(key));
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task<IReadOnlyCollection<string>?> GetAsync(string key)
    {
        try
        {
            var db = await GetDatabaseAsync();
            var members = await db.SetMembersAsync(FormatKey(key));
            return members.Select(m => (string)m!).ToArray();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<long?> GetLengthAsync(string key)
    {
        try
        {
            var db = await GetDatabaseAsync();
            return await db.SetLengthAsync(FormatKey(key));
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<IDatabase> GetDatabaseAsync()
    {
        if (_redis is { IsConnected: true })
        {
            return _redis.GetDatabase();
        }

        await _connectionLock.WaitAsync();
        try
        {
            if (_redis is not { IsConnected: true })
            {
                _redis?.Dispose();
                _redis = await ConnectionMultiplexer.ConnectAsync(_options.RedisConfiguration!);
            }

            return _redis.GetDatabase();
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private string FormatKey(string key)
    {
        return string.IsNullOrWhiteSpace(_options.KeyPrefix) ? key : $"{_options.KeyPrefix}_{key}";
    }

    public void Dispose()
    {
        _redis?.Dispose();
        _connectionLock.Dispose();
    }
}

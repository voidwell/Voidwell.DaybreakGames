using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.DaybreakGames.Cache;

public class Cache(IFusionCache cache, IListStore lists) : ICache
{
    private readonly IFusionCache _cache = cache;
    private readonly IListStore _lists = lists;

    public async Task SetAsync(string key, object value)
    {
        await _cache.SetAsync(key, value);
    }

    public async Task SetAsync(string key, object value, TimeSpan expires)
    {
        await _cache.SetAsync(key, value, new FusionCacheEntryOptions { Duration = expires });
    }

    public Task AddToListAsync(string key, string item)
    {
        return _lists.AddAsync(key, item);
    }

    public Task RemoveFromListAsync(string key, string item)
    {
        return _lists.RemoveAsync(key, item);
    }

    public Task ClearListAsync(string key)
    {
        return _lists.ClearAsync(key);
    }

    public async Task<IEnumerable<string>> GetListAsync(string key)
    {
        return await _lists.GetAsync(key) ?? Enumerable.Empty<string>();
    }

    public async Task<bool> TryGetListAsync(string key, Action<IEnumerable<string>> callback)
    {
        var items = await _lists.GetAsync(key);
        if (items == null)
        {
            return false;
        }

        callback(items);
        return true;
    }

    public async Task<long> GetListLengthAsync(string key)
    {
        return await _lists.GetLengthAsync(key) ?? 0;
    }

    public async Task<bool> TryGetListLengthAsync(string key, Action<long> callback)
    {
        var length = await _lists.GetLengthAsync(key);
        if (length == null)
        {
            return false;
        }

        callback(length.Value);
        return true;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var result = await _cache.TryGetAsync<T>(key);
        return result.GetValueOrDefault();
    }

    public async Task<bool> TryGetAsync<T>(string key, Action<T> callback)
    {
        var result = await _cache.TryGetAsync<T>(key);

        if (result.HasValue)
        {
            callback(result.Value);
            return true;
        }

        return false;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan expires, CancellationToken token = default)
    {
        return await _cache.GetOrSetAsync<T>(
            key,
            (_, ct) => factory(ct),
            new FusionCacheEntryOptions { Duration = expires },
            token: token);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan expires, Func<T, bool> cacheWhen, CancellationToken token = default)
    {
        return await _cache.GetOrSetAsync<T>(
            key,
            async (ctx, ct) =>
            {
                var value = await factory(ct);
                if (!cacheWhen(value))
                {
                    // A zero duration makes FusionCache skip caching this result
                    ctx.Options.Duration = TimeSpan.Zero;
                }

                return value;
            },
            new FusionCacheEntryOptions { Duration = expires },
            token: token);
    }

    public Task<T?> GetOrSetIfNotNullAsync<T>(string key, Func<CancellationToken, Task<T?>> factory, TimeSpan expires, CancellationToken token = default)
    {
        return GetOrSetAsync(key, factory, expires, value => value is not null, token);
    }
}

namespace Voidwell.DaybreakGames.Cache;

public interface ICache
{
    Task SetAsync(string key, object value);
    Task SetAsync(string key, object value, TimeSpan expires);
    Task<T> GetAsync<T>(string key);
    Task<bool> TryGetAsync<T>(string key, Action<T> callback);
    Task RemoveAsync(string key);

    /// <summary>
    /// Returns the cached value for the key, or runs the factory, caches its result for <paramref name="expires"/> and returns it.
    /// Concurrent callers for the same key share a single factory execution.
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan expires, CancellationToken token = default);

    /// <summary>
    /// Same as <see cref="GetOrSetAsync{T}(string, Func{CancellationToken, Task{T}}, TimeSpan, CancellationToken)"/>, but the factory result is only cached when <paramref name="cacheWhen"/> returns true.
    /// A result that is not cached is still returned to the caller.
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan expires, Func<T, bool> cacheWhen, CancellationToken token = default);

    /// <summary>
    /// Same as <see cref="GetOrSetAsync{T}(string, Func{CancellationToken, Task{T}}, TimeSpan, CancellationToken)"/>, but a <c>null</c> factory result is returned without being cached.
    /// </summary>
    Task<T?> GetOrSetIfNotNullAsync<T>(string key, Func<CancellationToken, Task<T?>> factory, TimeSpan expires, CancellationToken token = default);
    Task AddToListAsync(string key, string item);
    Task RemoveFromListAsync(string key, string item);
    Task ClearListAsync(string key);
    Task<IEnumerable<string>> GetListAsync(string key);
    Task<bool> TryGetListAsync(string key, Action<IEnumerable<string>> callback);
    Task<long> GetListLengthAsync(string key);
    Task<bool> TryGetListLengthAsync(string key, Action<long> callback);
}

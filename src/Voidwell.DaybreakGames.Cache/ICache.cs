namespace Voidwell.DaybreakGames.Cache;

public interface ICache
{
    Task SetAsync(string key, object value);
    Task SetAsync(string key, object value, TimeSpan expires);
    Task<T> GetAsync<T>(string key);
    Task<bool> TryGetAsync<T>(string key, Action<T> callback);
    Task RemoveAsync(string key);
    Task AddToListAsync(string key, string item);
    Task RemoveFromListAsync(string key, string item);
    Task<IEnumerable<string>> GetListAsync(string key);
    Task<bool> TryGetListAsync(string key, Action<IEnumerable<string>> callback);
    Task<long> GetListLengthAsync(string key);
    Task<bool> TryGetListLengthAsync(string key, Action<long> callback);
}

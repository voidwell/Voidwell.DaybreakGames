namespace Voidwell.DaybreakGames.Cache;

/// <summary>
/// Set-like list storage. FusionCache only handles single-value entries, so lists are stored separately.
/// </summary>
public interface IListStore
{
    Task AddAsync(string key, string item);
    Task RemoveAsync(string key, string item);
    Task ClearAsync(string key);
    Task<IReadOnlyCollection<string>?> GetAsync(string key);
    Task<long?> GetLengthAsync(string key);
}

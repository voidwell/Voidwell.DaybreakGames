using System.Collections.Concurrent;

namespace Voidwell.DaybreakGames.Cache;

/// <summary>
/// Process-local list storage used when Redis is not configured.
/// </summary>
public class MemoryListStore : IListStore
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _lists = new();

    public Task AddAsync(string key, string item)
    {
        var set = _lists.GetOrAdd(key, _ => new HashSet<string>());
        lock (set)
        {
            set.Add(item);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, string item)
    {
        if (_lists.TryGetValue(key, out var set))
        {
            lock (set)
            {
                set.Remove(item);
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<string>?> GetAsync(string key)
    {
        if (!_lists.TryGetValue(key, out var set))
        {
            return Task.FromResult<IReadOnlyCollection<string>?>(Array.Empty<string>());
        }

        lock (set)
        {
            return Task.FromResult<IReadOnlyCollection<string>?>(set.ToArray());
        }
    }

    public async Task<long?> GetLengthAsync(string key)
    {
        var items = await GetAsync(key);
        return items?.Count;
    }
}

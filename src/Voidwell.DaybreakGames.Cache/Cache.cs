using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Voidwell.DaybreakGames.Cache;

public class Cache : ICache
{
    private readonly ICacheConnector _connector;
    private readonly IOptions<CacheOptions> _options;

    public Cache(ICacheConnector connector, IOptions<CacheOptions> options)
    {
        _connector = connector;
        _options = options;
    }

    public async Task SetAsync(string key, object value)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            var sValue = JsonSerializer.Serialize(value);
            await db!.StringSetAsync(KeyFormatter(key), sValue);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task SetAsync(string key, object value, TimeSpan expires)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            var sValue = JsonSerializer.Serialize(value);
            await db!.StringSetAsync(KeyFormatter(key), sValue, expires);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task AddToListAsync(string key, string item)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            await db!.SetAddAsync(KeyFormatter(key), item);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task RemoveFromListAsync(string key, string item)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            await db!.SetRemoveAsync(KeyFormatter(key), item);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task<IEnumerable<string>> GetListAsync(string key)
    {
        var result = Enumerable.Empty<string>();

        await TryGetListAsync(key, value =>
        {
            result = value;
        });

        return result;
    }

    public async Task<bool> TryGetListAsync(string key, Action<IEnumerable<string>> callback)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            var list = await db!.SetMembersAsync(KeyFormatter(key));

            callback(list.ToStringArray()!);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<long> GetListLengthAsync(string key)
    {
        long result = 0;

        await TryGetListLengthAsync(key, length =>
        {
            result = length;
        });

        return result;
    }

    public async Task<bool> TryGetListLengthAsync(string key, Action<long> callback)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            callback(await db!.SetLengthAsync(KeyFormatter(key)));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<T> GetAsync<T>(string key)
    {
        T result = default!;

        await TryGetAsync<T>(key, value =>
        {
            result = value;
        });

        return result;
    }

    public async Task<bool> TryGetAsync<T>(string key, Action<T> callback)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            var value = await db!.StringGetAsync(KeyFormatter(key));

            callback(JsonSerializer.Deserialize<T>((string)value!)!);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task RemoveAsync(string key)
    {
        var db = await _connector.ConnectAsync();

        try
        {
            await db!.KeyDeleteAsync(KeyFormatter(key));
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private string KeyFormatter(string key)
    {
        if (!string.IsNullOrWhiteSpace(_options.Value.KeyPrefix))
        {
            return $"{_options.Value.KeyPrefix}_{key}";
        }

        return key;
    }
}

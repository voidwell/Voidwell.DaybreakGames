using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.Cache
{
    public class Cache : ICache, IDisposable
    {
        private readonly CacheOptions _options;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private ConnectionMultiplexer _redis;
        private IDatabase _db;

        public Cache(CacheOptions options)
        {
            _options = options;

            Task.Run(() => ConnectAsync());
        }

        public async Task SetAsync(string key, object value)
        {
            var db = await ConnectAsync();

            try
            {
                var sValue = JsonConvert.SerializeObject(value);
                await db.StringSetAsync(KeyFormatter(key), sValue);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async Task SetAsync(string key, object value, TimeSpan expires)
        {
            var db = await ConnectAsync();

            try
            {
                var sValue = JsonConvert.SerializeObject(value);
                await db.StringSetAsync(KeyFormatter(key), sValue, expiry: expires);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async Task AddToListAsync(string key, string item)
        {
            var db = await ConnectAsync();

            try
            {
                await db.SetAddAsync(KeyFormatter(key), item);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async Task RemoveFromListAsync(string key, string item)
        {
            var db = await ConnectAsync();

            try
            {
                await db.SetRemoveAsync(KeyFormatter(key), item);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async Task<IEnumerable<string>> GetListAsync(string key)
        {
            var db = await ConnectAsync();

            try
            {
                var list = await db.SetMembersAsync(KeyFormatter(key));

                return list.ToStringArray();
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }

        public async Task<long> GetListLengthAsync(string key)
        {
            var db = await ConnectAsync();

            try
            {
                return await db.SetLengthAsync(KeyFormatter(key));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = await ConnectAsync();

            try
            {
                var value = await db.StringGetAsync(KeyFormatter(key));

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch(Exception)
            {
                return default(T);
            }
        }

        public async Task RemoveAsync(string key)
        {
            var db = await ConnectAsync();

            try
            {
                await db.KeyDeleteAsync(KeyFormatter(key));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task<IDatabaseAsync> ConnectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_redis != null)
                return _db;

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_redis == null)
                    _redis = await ConnectionMultiplexer.ConnectAsync(_options.RedisConfiguration);

                _db = _redis.GetDatabase();
            }
            finally
            {
                _connectionLock.Release();
            }

            return _db;
        }

        private string KeyFormatter(string key)
        {
            return $"{_options.KeyPrefix}_{key}";
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}

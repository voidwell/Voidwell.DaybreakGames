using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace Voidwell.Cache
{
    public class Cache : ICache, IDisposable
    {
        private CacheOptions _options;
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
                return;
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
                return;
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
            catch(Exception)
            {
                return;
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

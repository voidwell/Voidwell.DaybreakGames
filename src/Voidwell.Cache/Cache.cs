using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Voidwell.Cache
{
    public class Cache : ICache, IDisposable
    {
        private CacheOptions _options;
        private ConnectionMultiplexer _redis;
        private IDatabase _db;

        public Cache(CacheOptions options)
        {
            _options = options;

            Task.Run(() => Connect());
        }

        public Task SetAsync(string key, object value)
        {
            try
            {
                var sValue = JsonConvert.SerializeObject(value);
                return _db.StringSetAsync(KeyFormatter(key), sValue);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
            
        }

        public Task SetAsync(string key, object value, TimeSpan expires)
        {
            try
            {
                var sValue = JsonConvert.SerializeObject(value);
                return _db.StringSetAsync(KeyFormatter(key), sValue, expiry: expires);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(KeyFormatter(key));

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch(Exception)
            {
                return default(T);
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                return _db.KeyDeleteAsync(KeyFormatter(key));
            }
            catch(Exception)
            {
                return Task.CompletedTask;
            }
        }

        private async Task Connect()
        {
            _redis = await ConnectionMultiplexer.ConnectAsync(_options.RedisConfiguration);
            _db = _redis.GetDatabase();
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

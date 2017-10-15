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
            var sValue = JsonConvert.SerializeObject(value);
            return _db.StringSetAsync(KeyFormatter(key), sValue);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var sValue = (string) await _db.StringGetAsync(KeyFormatter(key));
            return JsonConvert.DeserializeObject<T>(sValue);
        }

        public Task RemoveAsync(string key)
        {
            return _db.KeyDeleteAsync(KeyFormatter(key));
        }

        private async Task Connect()
        {
            _redis = await ConnectionMultiplexer.ConnectAsync(_options.RedisConfiguration);
            _db = _redis.GetDatabase();
        }

        private string KeyFormatter(string key)
        {
            return $"{_options.KeyPrefix}_key";
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}

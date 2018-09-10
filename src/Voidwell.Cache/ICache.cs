using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.Cache
{
    public interface ICache
    {
        Task SetAsync(string key, object value);
        Task SetAsync(string key, object value, TimeSpan expires);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task AddToListAsync(string key, string item);
        Task RemoveFromListAsync(string key, string item);
        Task<IEnumerable<string>> GetListAsync(string key);
        Task<long> GetListLengthAsync(string key);
    }
}

using System.Threading.Tasks;

namespace Voidwell.Cache
{
    public interface ICache
    {
        Task SetAsync(string key, object value);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}

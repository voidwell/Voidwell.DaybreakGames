using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census
{
    internal static class HttpExtensions
    {
        public static async Task<T> ReadAsObjectAsync<T>(this HttpContent content)
        {
            var serializedString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(serializedString, new JsonSerializerSettings {
                ContractResolver = new UnderscorePropertyNamesContractResolver()
            });
        }

        public static Task<T> GetContentAsync<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsObjectAsync<T>();
        }
    }
}

using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Voidwell.DaybreakGames.Utils
{
    public class JsonContent : StringContent
    {
        public JsonContent(string content) : base(content, Encoding.UTF8, "application/json")
        {
        }

        public static JsonContent FromObject(object objectToSerialize)
        {
            var serializedValue = JsonConvert.SerializeObject(objectToSerialize);
            return new JsonContent(serializedValue);
        }
    }
}

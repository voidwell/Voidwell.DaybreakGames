using Newtonsoft.Json;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class MultiLanguageString
    {
        [JsonProperty("en")]
        public string English { get;set; }
    }
}

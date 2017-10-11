using Newtonsoft.Json;

namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class MultiLanguageString
    {
        [JsonProperty("en")]
        public string English { get;set; }
    }
}

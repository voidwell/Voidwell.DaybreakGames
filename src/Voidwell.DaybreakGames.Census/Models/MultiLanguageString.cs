using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class MultiLanguageString
    {
        [JsonPropertyName("en")]
        public string English { get;set; }
    }
}

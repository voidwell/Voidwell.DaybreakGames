using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterRating
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    public double Rating { get; set; }
    public double Deviation { get; set; }
    public double Volatility { get; set; }

    public Character? Character { get; set; }
}

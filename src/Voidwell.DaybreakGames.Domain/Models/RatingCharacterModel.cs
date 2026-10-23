using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Domain.Models;

public class RatingCharacterModel
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long? CharacterId { get; set; }
    public string? Name { get; set; }
    public int? FactionId { get; set; }
    public int? WorldId { get; set; }
    public int? BattleRank { get; set; }
    public double Rating { get; set; }
    public double Deviation { get; set; }
}

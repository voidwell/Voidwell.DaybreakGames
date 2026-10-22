using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Domain.Models;

public class CharacterSearchResult
{
    public string? Name { get; set; }
    public int BattleRank { get; set; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long? Id { get; set; }
    public int FactionId { get; set; }
    public DateTime LastLogin { get; set; }
    public int WorldId { get; set; }
    public bool OnlineStatus { get; set; }
}

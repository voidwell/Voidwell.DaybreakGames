using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Repositories.Models;

public class CharacterLastSession
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    public string? Name { get; set; }
    public int? SessionId { get; set; }
    public DateTime? LoginDate { get; set; }
    public DateTime? LogoutDate { get; set; }
    public int? Duration { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class Outfit
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Alias { get; set; }
    public DateTime CreatedDate { get; set; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long? LeaderCharacterId { get; set; }
    public int MemberCount { get; set; }
    public int? FactionId { get; set; }
    public int? WorldId { get; set; }

    public Faction? Faction { get; set; }
    public World? World { get; set; }
    public Character? LeaderCharacter { get; set; }
}

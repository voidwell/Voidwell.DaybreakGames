using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events;

public class BattlerankUp
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
    [Required]
    public int BattleRank { get; set; }

    public int WorldId { get; set; }
    public int ZoneId { get; set; }
}

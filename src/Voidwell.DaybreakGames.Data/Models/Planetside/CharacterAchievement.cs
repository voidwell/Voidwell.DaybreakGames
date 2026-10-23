using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterAchievement
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public int AchievementId { get; set; }
    public int? EarnedCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }

    public Achievement? Achievement { get; set; }
}

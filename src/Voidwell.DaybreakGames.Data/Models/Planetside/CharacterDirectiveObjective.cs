using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterDirectiveObjective
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public int DirectiveId { get; set; }
    [Required]
    public int ObjectiveId { get; set; }
    public int ObjectiveGroupId { get; set; }
    public int Status { get; set; }
    public int? StateData { get; set; }

    public Objective? Objective { get; set; }
}

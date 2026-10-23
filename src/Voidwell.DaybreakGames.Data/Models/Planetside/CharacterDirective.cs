using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterDirective
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public int DirectiveId { get; set; }
    public int DirectiveTreeId { get; set; }
    public DateTime? CompletionTimeDate { get; set; }

    public IEnumerable<CharacterDirectiveObjective>? CharacterDirectiveObjectives { get; set; }
    public Directive? Directive { get; set; }
}

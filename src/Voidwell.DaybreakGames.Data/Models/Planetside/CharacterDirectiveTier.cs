using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterDirectiveTier
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public int DirectiveTreeId { get; set; }
    [Required]
    public int DirectiveTierId { get; set; }
    public DateTime? CompletionTimeDate { get; set; }

    public IEnumerable<CharacterDirective>? CharacterDirectives { get; set; }
}

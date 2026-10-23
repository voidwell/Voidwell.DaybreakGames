using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterDirectiveTree
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public int DirectiveTreeId { get; set; }
    public int CurrentDirectiveTierId { get; set; }
    public int CurrentLevel { get; set; }
    public DateTime? CompletionTimeDate { get; set; }

    public IEnumerable<CharacterDirectiveTier>? CharacterDirectiveTiers { get; set; }
}

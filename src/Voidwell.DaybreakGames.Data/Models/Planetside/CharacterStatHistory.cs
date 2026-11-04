using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterStatHistory
{
    [Required]
    public long CharacterId { get; set; }
    public string? StatName { get; set; }
    public int AllTime { get; set; }
    public int OneLifeMax { get; set; }
    public string? Day { get; set; }
    public string? Month { get; set; }
    public string? Week { get; set; }

    // The UTC date the history counts back from: Day[0] is this day, Week[0] its week and Month[0] its month.
    public DateTime? LastSaveDate { get; set; }

    public Character? Character { get; set; }
}

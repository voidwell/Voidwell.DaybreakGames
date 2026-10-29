using System.ComponentModel.DataAnnotations;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class World
{
    [Required]
    public int Id { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// False for legacy worlds that have been merged or shut down.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public Platform Platform { get; set; }
}

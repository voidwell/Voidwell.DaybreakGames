using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class OutfitMember
{
    [Required]
    public long CharacterId { get; set; }
    [Required]
    public long OutfitId { get; set; }

    public DateTime? MemberSinceDate { get; set; }
    public string? Rank { get; set; }
    public int? RankOrdinal { get; set; }

    public Character? Character { get; set; }
    public Outfit? Outfit { get; set; }
}

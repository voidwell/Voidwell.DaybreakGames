using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class OutfitMember
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long OutfitId { get; set; }

    public DateTime? MemberSinceDate { get; set; }
    public string? Rank { get; set; }
    public int? RankOrdinal { get; set; }

    public Character? Character { get; set; }
    public Outfit? Outfit { get; set; }
}

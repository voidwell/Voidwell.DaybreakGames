using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events;

public class PlayerFacilityCapture
{
    [Required]
    public long CharacterId { get; set; }
    [Required]
    public int FacilityId { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }

    public int WorldId { get; set; }
    public int ZoneId { get; set; }
    public long? OutfitId { get; set; }

    public MapRegion? Facility { get; set; }
}

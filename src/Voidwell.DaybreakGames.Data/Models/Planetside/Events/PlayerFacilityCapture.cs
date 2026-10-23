using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events;

public class PlayerFacilityCapture
{
    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }
    [Required]
    public int FacilityId { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }

    public int WorldId { get; set; }
    public int ZoneId { get; set; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long? OutfitId { get; set; }

    public MapRegion? Facility { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class PlayerSession
{
    public int Id { get; set; }

    [Required]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }

    public DateTime LoginDate { get; set; }
    public DateTime LogoutDate { get; set; }
    public int Duration { get; set; }
}

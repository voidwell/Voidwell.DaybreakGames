using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

[Table("CharacterUpdateQueue")]
public class CharacterUpdateQueue
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long CharacterId { get; set; }

    public DateTime Timestamp { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside;

[Table("CharacterUpdateQueue")]
public class CharacterUpdateQueue
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long CharacterId { get; set; }

    public DateTime Timestamp { get; set; }
}

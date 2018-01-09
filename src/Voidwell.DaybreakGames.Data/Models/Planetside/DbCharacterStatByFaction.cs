using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterStatByFaction")]
    public class DbCharacterStatByFaction
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ProfileId { get; set; }

        public int? KilledByVS { get; set; }
        public int? KilledByNC { get; set; }
        public int? KilledByTR { get; set; }
        public int? KillsVS { get; set; }
        public int? KillsNC { get; set; }
        public int? KillsTR { get; set; }

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
        public DbProfile Profile { get; set; }
    }
}

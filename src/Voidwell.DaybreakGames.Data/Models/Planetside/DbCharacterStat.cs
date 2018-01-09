using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterStat")]
    public class DbCharacterStat
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ProfileId { get; set; }

        public int? Deaths { get; set; }
        public int? FireCount { get; set; }
        public int? HitCount { get; set; }
        public int? PlayTime { get; set; }
        public int? Score { get; set; }

        // From By Faction
        public int? Kills { get; set; }
        public int? KilledBy { get; set; }

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
        public DbProfile Profile { get; set; }
    }
}

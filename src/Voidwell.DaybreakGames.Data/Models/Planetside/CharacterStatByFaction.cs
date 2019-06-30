using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterStatByFaction
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int ProfileId { get; set; }

        public int? KilledByVS { get; set; }
        public int? KilledByNC { get; set; }
        public int? KilledByTR { get; set; }
        public int? KillsVS { get; set; }
        public int? KillsNC { get; set; }
        public int? KillsTR { get; set; }

        public Profile Profile { get; set; }

        public Character Character { get; set; }
    }
}

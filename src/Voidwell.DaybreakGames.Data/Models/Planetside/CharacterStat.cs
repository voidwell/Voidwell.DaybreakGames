using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterStat
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int ProfileId { get; set; }

        public int? Deaths { get; set; }
        public int? FireCount { get; set; }
        public int? HitCount { get; set; }
        public int? PlayTime { get; set; }
        public int? Score { get; set; }
        public int? Kills { get; set; }
        public int? KilledBy { get; set; }

        public Profile Profile { get; set; }

        public Character Character { get; set; }
    }
}

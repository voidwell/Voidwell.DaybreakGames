using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Reward
    {
        [Required]
        public int Id { get; set; }
        public int RewardTypeId { get; set; }
        public int CountMin { get; set; }
        public int CountMax { get; set; }
        public int? Param1 { get; set; }
        public int? Param2 { get; set; }
        public int? Param3 { get; set; }
        public int? Param4 { get; set; }
        public int? Param5 { get; set; }

        public Item Item { get; set; }
    }
}

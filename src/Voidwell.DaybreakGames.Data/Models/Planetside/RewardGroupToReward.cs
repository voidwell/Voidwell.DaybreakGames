using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class RewardGroupToReward
    {
        [Required]
        public int RewardGroupId { get; set; }
        [Required]
        public int RewardId { get; set; }

        public Reward Reward { get; set; }
    }
}

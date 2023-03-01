using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class RewardSetToRewardGroup
    {
        [Required]
        public int RewardSetId { get; set; }
        [Required]
        public int RewardGroupId { get; set; }

        public IEnumerable<RewardGroupToReward> RewardGroups { get;set; }
    }
}

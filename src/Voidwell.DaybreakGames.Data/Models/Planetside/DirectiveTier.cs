using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class DirectiveTier
    {
        [Required]
        public int DirectiveTreeId { get; set; }
        [Required]
        public int DirectiveTierId { get; set; }
        public int? RewardSetId { get; set; }
        public int? DirectivePoints { get; set; }
        public int CompletionCount { get; set; }
        public string Name { get; set; }
        public int? ImageId { get; set; }
        public int? ImageSetId { get; set; }

        public IEnumerable<Directive> Directives { get; set; }
        public IEnumerable<RewardSetToRewardGroup> RewardGroupSets { get; set; }
    }
}

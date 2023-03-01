using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Achievement
    {
        [Required]
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? ObjectiveGroupId { get; set; }
        public int? RewardId { get; set; }
        public bool Repeatable { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ImageId { get; set; }

        public Objective Objective { get; set; }
    }
}

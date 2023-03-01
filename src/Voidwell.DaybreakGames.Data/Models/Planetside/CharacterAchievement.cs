using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterAchievement
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int AchievementId { get; set; }
        public int? EarnedCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public Achievement Achievement { get; set; }
    }
}

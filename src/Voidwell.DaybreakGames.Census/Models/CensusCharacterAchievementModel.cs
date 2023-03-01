using System;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusCharacterAchievementModel
    {
        public string CharacterId { get; set; }
        public int AchievementId { get; set;}
        public int? EarnedCount { get; set;}
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}

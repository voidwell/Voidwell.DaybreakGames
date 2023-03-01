namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusAchievementModel
    {
        public int AchievementId { get; set; }
        public int? ItemId { get; set; }
        public int? ObjectiveGroupId { get; set; }
        public int? RewardId { get; set; }
        public bool Repeatable { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int? ImageId { get; set; }
    }
}

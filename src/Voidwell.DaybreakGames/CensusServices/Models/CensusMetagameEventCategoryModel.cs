namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusMetagameEventCategoryModel
    {
        public int MetagameEventId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int Type { get; set; }
        public int ExperienceBonus { get; set; }
    }
}

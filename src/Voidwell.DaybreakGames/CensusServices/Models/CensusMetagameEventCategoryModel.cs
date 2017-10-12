namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusMetagameEventCategoryModel
    {
        public string MetagameEventId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public string Type { get; set; }
        public int ExperienceBonus { get; set; }
    }
}

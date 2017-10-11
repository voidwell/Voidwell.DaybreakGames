namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusFactionModel
    {
        public string FactionId { get; set; }
        public MultiLanguageString Name { get; set; }
        public string ImageId { get; set; }
        public string CodeTag { get; set; }
        public bool UserSelectable { get; set; }
    }
}

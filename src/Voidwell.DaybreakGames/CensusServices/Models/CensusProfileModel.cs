namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusProfileModel
    {
        public string ProfileId { get; set; }
        public string ProfileTypeId { get; set; }
        public string FactionId { get; set; }
        public MultiLanguageString Name { get; set; }
        public string ImageId { get; set; }
    }
}

namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusZoneModel
    {
        public string ZoneId { get; set; }
        public string Code { get; set; }
        public MultiLanguageString Name { get; set; }
        public int HexSize { get; set; }
        public MultiLanguageString Description { get; set; }
    }
}

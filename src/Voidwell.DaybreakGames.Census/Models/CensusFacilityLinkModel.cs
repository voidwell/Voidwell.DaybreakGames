namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusFacilityLinkModel
    {
        public int ZoneId { get; set; }
        public int FacilityIdA { get; set; }
        public int FacilityIdB { get; set; }
        public string Description { get; set; }
    }
}

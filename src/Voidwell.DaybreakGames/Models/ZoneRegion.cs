namespace Voidwell.DaybreakGames.Models
{
    public class ZoneRegion
    {
        public int RegionId { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Z { get; set; }
    }
}

namespace Voidwell.DaybreakGames.Models
{
    public class ZoneRegion
    {
        public int RegionId { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public float? XPos { get; set; }
        public float? YPos { get; set; }
        public float? ZPos { get; set; }
    }
}

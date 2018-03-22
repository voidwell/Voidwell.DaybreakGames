namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusMapRegionModel
    {
        public int MapRegionId { get; set; }
        public int ZoneId { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
        public float? LocationX { get; set; }
        public float? LocationY { get; set; }
        public float? LocationZ { get; set; }
    }
}

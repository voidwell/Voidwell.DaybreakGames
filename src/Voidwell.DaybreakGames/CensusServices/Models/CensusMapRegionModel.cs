namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusMapRegionModel
    {
        public string MapRegionId { get; set; }
        public string ZoneId { get; set; }
        public string FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public float LocationZ { get; set; }
    }
}

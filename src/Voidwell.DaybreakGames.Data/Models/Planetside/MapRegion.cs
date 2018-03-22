using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class MapRegion
    {
        [Required]
        public int Id { get; set; }

        public int ZoneId { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
        public float? XPos { get; set; }
        public float? YPos { get; set; }
        public float? ZPos { get; set; }
    }
}

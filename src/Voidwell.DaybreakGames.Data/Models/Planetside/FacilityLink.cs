using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class FacilityLink
    {
        [Required]
        public int ZoneId { get; set; }
        [Required]
        public int FacilityIdA { get; set; }
        [Required]
        public int FacilityIdB { get; set; }
        public string Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class FacilityLink
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public int ZoneId { get; set; }
        public int FacilityIdA { get; set; }
        public int FacilityIdB { get; set; }
        public string Description { get; set; }
    }
}

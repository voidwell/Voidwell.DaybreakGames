using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("FacilityLink")]
    public class DbFacilityLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string ZoneId { get; set; }
        public string FacilityIdA { get; set; }
        public string FacilityIdB { get; set; }
        public string Description { get; set; }
    }
}

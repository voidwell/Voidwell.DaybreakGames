using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("MetagameEventCategory")]
    public class DbMetagameEventCategory
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int ExperienceBonus { get; set; }
    }
}

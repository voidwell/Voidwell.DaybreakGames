using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Zone")]
    public class DbZone
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int HexSize { get; set; }
    }
}

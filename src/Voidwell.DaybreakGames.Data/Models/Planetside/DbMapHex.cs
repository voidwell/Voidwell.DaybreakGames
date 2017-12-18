using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("MapHex")]
    public class DbMapHex
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string ZoneId { get; set; }
        [Required]
        public string MapRegionId { get; set; }

        public int XPos { get; set; }
        public int YPos { get; set; }
        public string HexType { get; set; }
        public string TypeName { get; set; }
    }
}

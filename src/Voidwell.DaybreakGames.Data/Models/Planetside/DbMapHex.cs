using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("MapHex")]
    public class DbMapHex
    {
        [Required]
        public string Id { get; set; }

        public string ZoneId { get; set; }
        public string MapRegionId { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public string HexType { get; set; }
        public string TypeName { get; set; }
    }
}

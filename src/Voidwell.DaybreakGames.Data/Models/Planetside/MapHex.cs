using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class MapHex
    {
        public int Id { get; set; }

        [Required]
        public int ZoneId { get; set; }
        [Required]
        public int MapRegionId { get; set; }

        public int XPos { get; set; }
        public int YPos { get; set; }
        public int HexType { get; set; }
        public string TypeName { get; set; }
    }
}

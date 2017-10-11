using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("VehicleFaction")]
    public class DbVehicleFaction
    {
        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string FactionId { get; set; }
    }
}

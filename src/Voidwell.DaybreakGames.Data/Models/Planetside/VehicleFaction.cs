using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class VehicleFaction
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int FactionId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}

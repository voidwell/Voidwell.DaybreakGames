using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Vehicle")]
    public class DbVehicle
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string CostResourceId { get; set; }
        public string ImageId { get; set; }

        public IEnumerable<DbVehicleFaction> Faction { get; set; }
    }
}

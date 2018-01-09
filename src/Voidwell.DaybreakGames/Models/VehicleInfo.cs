using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class VehicleInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageId { get; set; }
        public IEnumerable<string> Factions { get; set; }
    }
}

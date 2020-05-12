using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;

namespace Voidwell.DaybreakGames.Messaging.Models
{
    public class PlanetsideMessage
    {
        public PlanetsideMessage()
        {
            Timestamp = DateTime.UtcNow;
        }

        public DateTime Timestamp { get; set; }
        public int WorldId { get; set; }
        public string WorldName { get; set; }
        public int? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return JToken.FromObject(this).ToString(Formatting.None);
        }
    }
}

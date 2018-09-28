using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;

namespace Voidwell.DaybreakGames.WebsocketServer.Models
{
    public class PlanetsideMessage
    {
        public PlanetsideMessage()
        {
            Timestamp = DateTime.UtcNow;
        }

        public DateTime Timestamp { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return JToken.FromObject(this).ToString(Formatting.None);
        }
    }
}

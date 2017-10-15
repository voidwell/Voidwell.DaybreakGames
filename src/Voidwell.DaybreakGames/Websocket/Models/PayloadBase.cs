using System;

namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class PayloadBase
    {
        public string EventName { get; set; }
        public string WorldId { get; set; }
        public string ZoneId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

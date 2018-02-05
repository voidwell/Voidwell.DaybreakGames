using System;

namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class PayloadBase
    {
        public string EventName { get; set; }
        public int WorldId { get; set; }
        public int? ZoneId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

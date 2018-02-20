using System;

namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class CensusHeartbeat
    {
        public DateTime LastHeartbeat { get; set; }
        public object Contents { get; set; }
    }
}

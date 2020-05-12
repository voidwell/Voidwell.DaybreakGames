using System;

namespace Voidwell.DaybreakGames.GameState.CensusStream.Models
{
    public class CensusHeartbeat
    {
        public DateTime LastHeartbeat { get; set; }
        public object Contents { get; set; }
    }
}

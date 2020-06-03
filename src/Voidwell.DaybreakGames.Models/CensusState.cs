using System;

namespace Voidwell.DaybreakGames.Models
{
    public class CensusState
    {
        public DateTime LastStateChange { get; set; }
        public object Contents { get; set; }
        public object Health { get; set; }
    }
}

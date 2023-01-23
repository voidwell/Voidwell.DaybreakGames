using System;

namespace Voidwell.DaybreakGames.Live.CensusStream.Models
{
    public class CensusState
    {
        public DateTime LastStateChange { get; set; }
        public object Contents { get; set; }
    }
}

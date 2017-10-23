using System.Collections.Generic;

namespace Voidwell.DaybreakGames
{
    public class DaybreakGamesOptions
    {
        public string CensusServiceKey { get; set; }
        public IEnumerable<string> CensusWebsocketWorlds { get; set; }
        public IEnumerable<string> CensusWebsocketServices { get; set; }
    }
}

using System.Collections.Generic;

namespace Voidwell.DaybreakGames
{
    public class DaybreakGamesOptions
    {
        public string CensusServiceKey { get; set; } = "example";
        public IEnumerable<string> CensusWebsocketWorlds { get; set; }
        public IEnumerable<string> CensusWebsocketServices { get; set; }
        public IEnumerable<string> CensusWebsocketCharacters { get; set; }
        public bool AutostartWebsocketMonitor { get; set; } = false;
        public bool DisableUpdater { get; set; } = false;
        public bool DisableCharacterUpdater { get; set; } = false;
    }
}

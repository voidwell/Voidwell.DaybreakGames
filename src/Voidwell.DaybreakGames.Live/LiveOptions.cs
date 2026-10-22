namespace Voidwell.DaybreakGames.Live;

public class LiveOptions
{
    public bool DisableCharacterUpdater { get; set; } = false;
    public bool DisableCensusMonitor { get; set; } = false;
    public IEnumerable<string>? CensusWebsocketWorlds { get; set; }
    public IEnumerable<string>? CensusWebsocketServices { get; set; }
    public IEnumerable<string>? CensusWebsocketCharacters { get; set; }
}

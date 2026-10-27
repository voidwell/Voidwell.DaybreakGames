using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Live.CensusStream;

public interface IWebsocketHealthMonitor
{
    bool IsHealthy(Platform platform);
    void ReceivedEvent(Platform platform, int worldId, string eventName, DateTime? timestamp = null);
    void ClearWorld(int worldId);
    void ClearAllWorlds(Platform platform);
}

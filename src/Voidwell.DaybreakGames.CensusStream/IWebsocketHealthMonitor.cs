using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IWebsocketHealthMonitor
    {
        bool IsHealthy();
        void ReceivedEvent(int worldId, string eventName);
        void ReceivedEvent(int worldId, string eventName, DateTime timestamp);
        void ClearWorld(int worldId);
        void ClearAllWorlds();
        Dictionary<int, Dictionary<string, DateTime>> GetHealthState();
    }
}
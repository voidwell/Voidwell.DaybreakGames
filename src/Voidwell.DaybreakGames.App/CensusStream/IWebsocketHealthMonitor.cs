﻿using System;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IWebsocketHealthMonitor
    {
        bool IsHealthy();
        void ReceivedEvent(int worldId, string eventName, DateTime? timestamp = null);
        void ClearWorld(int worldId);
        void ClearAllWorlds();
    }
}
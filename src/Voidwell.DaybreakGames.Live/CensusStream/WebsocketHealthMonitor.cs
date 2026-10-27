using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Live.CensusStream;

public class WebsocketHealthMonitor : IWebsocketHealthMonitor
{
    private readonly ILogger<WebsocketHealthMonitor> _logger;

    // Each platform has its own stream, so health is tracked and cleared per platform
    private readonly ConcurrentDictionary<Platform, ConcurrentDictionary<int, ConcurrentDictionary<string, DateTime>>> _platformWorldsLastSeenEvents = new();

    private readonly List<int> _ignorableWorlds = new List<int> { 19, 25 };
    private readonly Dictionary<string, TimeSpan> _unhealthyEventIntervals = new Dictionary<string, TimeSpan>
    {
        {  "Death", TimeSpan.FromMinutes(5) }
    };

    public WebsocketHealthMonitor(ILogger<WebsocketHealthMonitor> logger)
    {
        _logger = logger;
    }

    public void ReceivedEvent(Platform platform, int worldId, string eventName, DateTime? timestamp = null)
    {
        if (timestamp == null)
        {
            timestamp = DateTime.UtcNow;
        }

        try
        {
            var worldsLastSeenEvents = _platformWorldsLastSeenEvents.GetOrAdd(platform, _ => new());
            if (!worldsLastSeenEvents.ContainsKey(worldId))
            {
                worldsLastSeenEvents.TryAdd(worldId, new ConcurrentDictionary<string, DateTime>());
            }

            worldsLastSeenEvents[worldId].AddOrUpdate(eventName, timestamp.Value, (k, v) => timestamp.Value);
        }
        catch (Exception)
        { }
    }

    public void ClearWorld(int worldId)
    {
        foreach (var worldsLastSeenEvents in _platformWorldsLastSeenEvents.Values)
        {
            worldsLastSeenEvents.TryRemove(worldId, out var _);
        }
    }

    public void ClearAllWorlds(Platform platform)
    {
        _platformWorldsLastSeenEvents.TryRemove(platform, out var _);
    }

    public bool IsHealthy(Platform platform)
    {
        if (!_platformWorldsLastSeenEvents.TryGetValue(platform, out var worldsLastSeenEvents))
        {
            return true;
        }

        var worldIds = worldsLastSeenEvents.Keys.Where(a => !_ignorableWorlds.Contains(a)).ToList();

        return !worldIds.Where(a => !TryEvaluateWorldHealth(worldsLastSeenEvents, a)).Any();
    }

    private bool TryEvaluateWorldHealth(ConcurrentDictionary<int, ConcurrentDictionary<string, DateTime>> worldsLastSeenEvents, int worldId)
    {
        if (worldsLastSeenEvents.TryGetValue(worldId, out var eventList))
        {
            if (!EvaluateWorldHealth(eventList))
            {
                _logger.LogWarning(34214, "Stream for world '{worldId}' failed health check", worldId);
                return false;
            }
        }
        return true;
    }

    private bool EvaluateWorldHealth(ConcurrentDictionary<string, DateTime> worldEvents)
    {
        var now = DateTime.UtcNow;

        foreach ((var eventName, var interval) in _unhealthyEventIntervals)
        {
            if (worldEvents != null && worldEvents.TryGetValue(eventName, out var lastReceivedTime))
            {
                if (now - lastReceivedTime > interval)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

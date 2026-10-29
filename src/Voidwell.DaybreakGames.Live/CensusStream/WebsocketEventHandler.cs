using System.Text.Json;
using System.Text.RegularExpressions;
using DaybreakGames.Census;
using Microsoft.Extensions.Logging;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Live.CensusStream;

public class WebsocketEventHandler : IWebsocketEventHandler
{
    private readonly IEventProcessorHandler _processorHandler;
    private readonly IWorldMonitor _worldMonitor;
    private readonly IAlertService _alertService;
    private readonly IMetagameEventService _metagameEventService;
    private readonly ICensusContext _censusContext;
    private readonly ILogger<WebsocketEventHandler> _logger;

    private const string _regServer = @"EventServerEndpoint_(.*)_(.*)";

    public WebsocketEventHandler(IEventProcessorHandler processorHandler, IWorldMonitor worldMonitor,
        IAlertService alertService, IMetagameEventService metagameEventService, ICensusContext censusContext,
        ILogger<WebsocketEventHandler> logger)
    {
        _processorHandler = processorHandler;
        _worldMonitor = worldMonitor;
        _alertService = alertService;
        _metagameEventService = metagameEventService;
        _censusContext = censusContext;
        _logger = logger;
    }

    public async Task Process(Platform platform, JsonElement message)
    {
        // Everything the message triggers, down to character and map lookups, queries the platform it came from
        using var platformScope = _censusContext.Use(platform);

        if (message.TryGetString("type") == "serviceStateChanged")
        {
            try
            {
                await ProcessServiceStateChangedAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(75843, ex, "Failed to process service state change.");
            }

            return;
        }

        await ProcessServiceEventAsync(platform, message);
    }

    private async Task ProcessServiceStateChangedAsync(JsonElement message)
    {
        var detail = message.TryGetString("detail");

        var r = new Regex(_regServer);
        var m = r.Match(detail!);

        var worldName = m.Groups[1].Value;

        if (int.TryParse(m.Groups[2].Value, out var worldId))
        {
            var isWorldOnline = bool.TryParse(message.TryGetString("online"), out bool result) ? result : false;

            await _worldMonitor.SetWorldState(worldId, worldName, isWorldOnline);

            if (isWorldOnline)
            {
                var activeAlerts = await _alertService.GetActiveAlertsByWorldId(worldId);
                if (activeAlerts != null)
                {
                    foreach (var alert in activeAlerts)
                    {
                        var metagameEvent = await _metagameEventService.GetMetagameEvent(alert.MetagameEventId!.Value);
                        var alertState = new ZoneAlertState(alert.StartDate!.Value, alert.MetagameInstanceId, metagameEvent!);
                        _worldMonitor.UpdateZoneAlert(alert.WorldId, alert.ZoneId!.Value, alertState);
                    }
                }
            }
        }
    }

    private async Task ProcessServiceEventAsync(Platform platform, JsonElement message)
    {
        var jPayload = message.GetProperty("payload");

        var payload = jPayload.Deserialize<PayloadBase>(StreamConstants.SerializerOptions);

        var eventName = payload?.EventName;

        if (payload == null || eventName == null)
        {
            return;
        }

        _logger.LogDebug("Payload received for event: {0}.", eventName);

        if (payload.ZoneId.HasValue && payload.ZoneId.Value > 1000)
        {
            return;
        }

        try
        {
            if (!await _processorHandler.TryProcessAsync(eventName, jPayload))
            {
                _logger.LogWarning("No process method found for event: {0}", eventName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(75642, ex, "Failed to process websocket event: {0}.", eventName);
        }
    }
}

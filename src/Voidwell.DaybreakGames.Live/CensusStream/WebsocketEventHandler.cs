using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using System.Text.Json;
using DaybreakGames.Census;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        private readonly IEventProcessorHandler _processorHandler;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IAlertService _alertService;
        private readonly IMetagameEventService _metagameEventService;
        private readonly IWebsocketHealthMonitor _healthMonitor;
        private readonly ILogger<WebsocketEventHandler> _logger;

        private const string RegServer = @"EventServerEndpoint_(.*)_(.*)";

        public WebsocketEventHandler(IEventProcessorHandler processorHandler, IWorldMonitor worldMonitor,
            IAlertService alertService, IMetagameEventService metagameEventService, IWebsocketHealthMonitor healthMonitor,
            ILogger<WebsocketEventHandler> logger)
        {
            _processorHandler = processorHandler;
            _worldMonitor = worldMonitor;
            _alertService = alertService;
            _metagameEventService = metagameEventService;
            _healthMonitor = healthMonitor;
            _logger = logger;
        }

        public async Task Process(JsonElement message)
        {
            if (message.TryGetString("type") == "serviceStateChanged")
            {
                try
                {
                    await ProcessServiceStateChanged(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(75843, ex, "Failed to process service state change.");
                }

                return;
            }

            await ProcessServiceEvent(message);
        }

        private async Task ProcessServiceStateChanged(JsonElement message)
        {
            var detail = message.TryGetString("detail");

            var r = new Regex(RegServer);
            var m = r.Match(detail);

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
                        foreach(var alert in activeAlerts)
                        {
                            var metagameEvent = await _metagameEventService.GetMetagameEvent(alert.MetagameEventId.Value);
                            var alertState = new ZoneAlertState(alert.StartDate.Value, alert.MetagameInstanceId, metagameEvent);
                            _worldMonitor.UpdateZoneAlert(alert.WorldId, alert.ZoneId.Value, alertState);
                        }
                    }
                }
                else
                {
                    _healthMonitor.ClearWorld(worldId);
                }
            }
        }

        private async Task ProcessServiceEvent(JsonElement message)
        {
            var jPayload = message.GetProperty("payload");

            var payload = jPayload.Deserialize<PayloadBase>(StreamConstants.SerializerOptions);

            var eventName = payload?.EventName;

            _healthMonitor.ReceivedEvent(payload.WorldId, eventName);

            if (eventName == null)
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
}

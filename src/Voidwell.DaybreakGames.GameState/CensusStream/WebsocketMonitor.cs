using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.GameState.CensusStream.Models;
using Voidwell.DaybreakGames.Core;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.GameState.Services;

namespace Voidwell.DaybreakGames.GameState.CensusStream
{
    public class WebsocketMonitor : IWebsocketMonitor, IDisposable
    {
        private readonly ICensusStreamClient _client;
        private readonly IWebsocketEventHandler _handler;
        private readonly DaybreakGamesOptions _options;
        private readonly IWorldMonitor _worldMonitor;
        private readonly ILogger<WebsocketMonitor> _logger;

        private CensusHeartbeat _lastHeartbeat;

        public WebsocketMonitor(ICensusStreamClient censusStreamClient, IWebsocketEventHandler handler, IOptions<DaybreakGamesOptions> options,
            IWorldMonitor worldMonitor, ILogger<WebsocketMonitor> logger)
        {
            _client = censusStreamClient;
            _handler = handler;
            _options = options.Value;
            _worldMonitor = worldMonitor;
            _logger = logger;

            _client.Subscribe(CreateSubscription())
                .OnMessage(OnMessage)
                .OnDisconnect(OnDisconnect);
        }

        private CensusStreamSubscription CreateSubscription()
        {
            var eventNames = new List<string>();

            if (_options.CensusWebsocketServices != null)
            {
                eventNames.AddRange(_options.CensusWebsocketServices);
            }

            if (_options.CensusWebsocketExperienceIds != null && _options.CensusWebsocketExperienceIds.Any())
            {
                //var experienceEvents = _options.CensusWebsocketExperienceIds.Select(id => $"GainExperience_experience_id_{id}");
                //eventNames.AddRange(experienceEvents);
            }

            return new CensusStreamSubscription
            {
                Characters = _options.CensusWebsocketCharacters,
                Worlds = _options.CensusWebsocketWorlds,
                EventNames = eventNames
            };
        }

        public CensusHeartbeat GetLastHeartbeat()
        {
            return _lastHeartbeat;
        }

        public async Task ConnectAsync()
        {
            await _client?.ConnectAsync();
        }

        public async Task DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }

        private async Task OnMessage(string message)
        {
            if (message == null)
            {
                return;
            }

            JToken msg;

            try
            {
                msg = JToken.Parse(message);
            }
            catch(Exception)
            {
                _logger.LogError(91097, "Failed to parse message: {0}", message);
                return;
            }

            if (msg.Value<string>("type") == "heartbeat")
            {
                _lastHeartbeat = new CensusHeartbeat
                {
                    LastHeartbeat = DateTime.UtcNow,
                    Contents = msg.ToObject<object>()
                };

                return;
            }

            #pragma warning disable CS4014
            Task.Run(() =>
            {
                _handler.Process(msg);
            }).ConfigureAwait(false);
            #pragma warning restore CS4014
        }

        private async Task OnDisconnect(string error)
        {
            await _worldMonitor.ClearAllWorldStates();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}

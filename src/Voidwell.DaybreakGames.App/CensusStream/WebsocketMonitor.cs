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
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.CensusStream.Models;

namespace Voidwell.DaybreakGames.CensusStream
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly ICensusStreamClient _client;
        private readonly IWebsocketEventHandler _handler;
        private readonly DaybreakGamesOptions _options;
        private readonly IWorldMonitor _worldMonitor;
        private readonly ILogger<WebsocketMonitor> _logger;

        private CensusState _lastStateChange;

        public override string ServiceName => "CensusMonitor";

        public WebsocketMonitor(ICensusStreamClient censusStreamClient, IWebsocketEventHandler handler, IOptions<DaybreakGamesOptions> options,
            IWorldMonitor worldMonitor, ICache cache, ILogger<WebsocketMonitor> logger)
                :base(cache)
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

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableCensusMonitor)
            {
                _logger.LogInformation("Census monitor is disabled");
                return;
            }

            _logger.LogInformation("Starting census stream monitor");

            try
            {
                await _client.ConnectAsync();
            }
            catch(Exception ex)
            {
                await _client?.DisconnectAsync();
                await UpdateStateAsync(false);
                UpdateStateDetails(ex.Message);

                _logger.LogError(91435, ex, "Failed to establish initial connection to Census. Will not attempt to reconnect.");
            }
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            if (_client == null)
            {
                return;
            }

            await _client?.DisconnectAsync();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _client?.DisconnectAsync();
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_lastStateChange);
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
                UpdateStateDetails(msg.ToObject<object>());
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
            UpdateStateDetails(error);
            await _worldMonitor.ClearAllWorldStates();
        }

        private void UpdateStateDetails(object contents)
        {
            _lastStateChange = new CensusState
            {
                LastStateChange = DateTime.UtcNow,
                Contents = contents
            };
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}

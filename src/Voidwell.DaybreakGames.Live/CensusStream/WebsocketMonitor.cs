﻿using DaybreakGames.Census.Stream;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Utils.HostedService;
using Websocket.Client;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public class WebsocketMonitor : IStatefulHostedService, IDisposable
    {
        private readonly ICensusStreamClient _client;
        private readonly IWebsocketEventHandler _handler;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IWebsocketHealthMonitor _healthMonitor;
        private readonly LiveOptions _options;
        private readonly HostedServiceState<WebsocketMonitor> _state;
        private readonly ILogger<WebsocketMonitor> _logger;

        private CensusState _lastStateChange;
        private Timer _timer;

        public WebsocketMonitor(ICensusStreamClient streamClient, IWebsocketEventHandler handler, 
            IWorldMonitor worldMonitor, IWebsocketHealthMonitor healthMonitor, IOptions<LiveOptions> options,
            HostedServiceState<WebsocketMonitor> state, ILogger<WebsocketMonitor> logger)
        {
            _client = streamClient;
            _handler = handler;
            _worldMonitor = worldMonitor;
            _healthMonitor = healthMonitor;
            _options = options.Value;
            _state = state;
            _logger = logger;

            _client.OnConnect(OnConnect)
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableCensusMonitor)
            {
                _logger.LogInformation("Census monitor is disabled");
                return;
            }

            _logger.LogInformation("Starting census stream monitor");

            await _client.ConnectAsync();

            _timer = new Timer(CheckDataHealth, null, 0, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            if (_client == null)
            {
                return;
            }

            _timer?.Dispose();
            await _client?.DisconnectAsync();
            _healthMonitor.ClearAllWorlds();
        }

        public Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _client?.DisconnectAsync();
        }

        public Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_lastStateChange);
        }

        private Task OnConnect(ReconnectionType type)
        {
            _client.Subscribe(CreateSubscription());
            return Task.CompletedTask;
        }

        private Task OnMessage(string message)
        {
            if (message == null)
            {
                return Task.CompletedTask;
            }

            JsonElement msg;

            try
            {
                msg = JsonSerializer.Deserialize<JsonElement>(message);
            }
            catch(Exception)
            {
                _logger.LogError(91097, "Failed to parse message: {0}", message);
                return Task.CompletedTask;
            }

            if (msg.TryGetString("type") == "heartbeat")
            {
                UpdateStateDetails(msg.Deserialize<object>());
                return Task.CompletedTask;
            }

            #pragma warning disable CS4014
            Task.Run(() =>
            {
                _handler.Process(msg);
            });
            #pragma warning restore CS4014

            return Task.CompletedTask;
        }

        private async Task OnDisconnect(DisconnectionInfo info)
        {
            UpdateStateDetails(info.Exception?.Message ?? info.Type.ToString());
            await _worldMonitor.ClearAllWorldStates();
            _healthMonitor.ClearAllWorlds();
        }

        private void UpdateStateDetails(object contents)
        {
            _lastStateChange = new CensusState
            {
                LastStateChange = DateTime.UtcNow,
                Contents = contents
            };
        }

        private async void CheckDataHealth(object state)
        {
            if (!_state.IsRunning)
            {
                _healthMonitor.ClearAllWorlds();
                _timer?.Dispose();
                return;
            }

            if (!_healthMonitor.IsHealthy())
            {
                _logger.LogError(45234, "Census stream has failed health checks. Attempting resetting connection.");

                try
                {
                    await _client?.ReconnectAsync();
                }
                catch (Exception ex)
                {
                    UpdateStateDetails(ex.Message);

                    _logger.LogError(45235, ex, "Failed to reestablish connection to Census");
                }
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            _timer?.Dispose();
        }
    }
}

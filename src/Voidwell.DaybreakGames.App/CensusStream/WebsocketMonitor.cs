using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly ICensusStreamClient _deathClient;
        private readonly ICensusStreamClient _loginLogoutClient;
        private readonly ICensusStreamClient _otherClient;

        private readonly IWebsocketEventHandler _handler;
        private readonly DaybreakGamesOptions _options;
        private readonly IWorldMonitor _worldMonitor;
        private readonly ILogger<WebsocketMonitor> _logger;

        private Dictionary<string, CensusHeartbeat> _lastHeartbeat = new Dictionary<string, CensusHeartbeat>();

        public override string ServiceName => "CensusMonitor";

        private static readonly string[] _deathClientServices = { "Death" };
        private static readonly string[] _loginLogoutClientServices = { "PlayerLogin", "PlayerLogout" };

        public WebsocketMonitor(IServiceProvider provider, IWebsocketEventHandler handler, IOptions<DaybreakGamesOptions> options,
            IWorldMonitor worldMonitor, ICache cache, ILogger<WebsocketMonitor> logger)
                :base(cache)
        {
            _deathClient = provider.GetService<ICensusStreamClient>();
            _loginLogoutClient = provider.GetService<ICensusStreamClient>();
            _otherClient = provider.GetService<ICensusStreamClient>();

            _handler = handler;
            _options = options.Value;
            _worldMonitor = worldMonitor;
            _logger = logger;

            var censusServices = _options.CensusWebsocketServices;

            _deathClient.Subscribe(CreateSubscription(censusServices.Where(a => _deathClientServices.Contains(a))))
                .OnMessage(a => OnMessage("DeathClient", a))
                .OnDisconnect(OnDisconnect);

            _loginLogoutClient.Subscribe(CreateSubscription(censusServices.Where(a => _loginLogoutClientServices.Contains(a))))
                .OnMessage(a => OnMessage("LoginLogoutClient", a))
                .OnDisconnect(OnDisconnect);

            _otherClient.Subscribe(CreateSubscription(censusServices.Where(a => !_deathClientServices.Contains(a) && !_loginLogoutClientServices.Contains(a))))
                .OnMessage(a => OnMessage("OtherClient", a))
                .OnDisconnect(OnDisconnect);
        }

        private CensusStreamSubscription CreateSubscription(IEnumerable<string> services)
        {
            var eventNames = new List<string>();

            if (services != null)
            {
                eventNames.AddRange(services);
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
                await _otherClient.ConnectAsync();
                await _loginLogoutClient.ConnectAsync();
                await _deathClient.ConnectAsync();
            }
            catch(Exception ex)
            {
                await DisconnectAllClientsAsync();
                await UpdateStateAsync(false);

                _logger.LogError(91435, ex, "Failed to establish initial connection to Census. Will not attempt to reconnect.");
            }
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");
            
            await DisconnectAllClientsAsync();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await DisconnectAllClientsAsync();
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_lastHeartbeat);
        }

        private async Task DisconnectAllClientsAsync()
        {
            await _deathClient?.DisconnectAsync();
            await _loginLogoutClient?.DisconnectAsync();
            await _otherClient?.DisconnectAsync();
        }

        private async Task OnMessage(string subject, string message)
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
                _lastHeartbeat[subject] = new CensusHeartbeat
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
            _deathClient?.Dispose();
            _loginLogoutClient?.Dispose();
            _otherClient?.Dispose();
        }
    }
}

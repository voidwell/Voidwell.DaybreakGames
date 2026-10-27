using System.Text.Json;
using DaybreakGames.Census;
using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Utils;
using Voidwell.DaybreakGames.Utils.HostedService;
using Websocket.Client;

namespace Voidwell.DaybreakGames.Live.CensusStream;

/// <summary>
/// Streams the events of one platform. One monitor runs per enabled platform.
/// </summary>
public class WebsocketMonitor : IStatefulHostedService, IDisposable
{
    private readonly Platform _platform;
    private readonly ICensusStreamClient _client;
    private readonly IWebsocketEventHandler _handler;
    private readonly IWorldMonitor _worldMonitor;
    private readonly IWebsocketHealthMonitor _healthMonitor;
    private readonly LiveOptions _options;
    private readonly HostedServiceState _state;
    private readonly ILogger<WebsocketMonitor> _logger;

    private CensusState? _lastStateChange;
    private Timer? _timer;

    public WebsocketMonitor(Platform platform, ICensusStreamClient streamClient, IWebsocketEventHandler handler,
        IWorldMonitor worldMonitor, IWebsocketHealthMonitor healthMonitor, IOptions<LiveOptions> options,
        HostedServiceState state, ILogger<WebsocketMonitor> logger)
    {
        _platform = platform;
        _client = streamClient;
        _handler = handler;
        _worldMonitor = worldMonitor;
        _healthMonitor = healthMonitor;
        _options = options.Value;
        _state = state;
        _logger = logger;

        _client.OnConnect(OnConnect)
            .OnMessage(OnMessage)
            .OnDisconnect(OnDisconnectAsync);
    }

    private CensusStreamSubscription CreateSubscription()
    {
        var eventNames = new List<string>();

        if (_options.CensusWebsocketServices != null)
        {
            eventNames.AddRange(_options.CensusWebsocketServices);
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
            _logger.LogInformation("Census monitor is disabled for {Platform}", _platform);
            return;
        }

        _logger.LogInformation("Starting census stream monitor for {Platform}", _platform);

        await _client.ConnectAsync();

        _timer = new Timer(CheckDataHealthAsync, null, 0, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping census stream monitor for {Platform}", _platform);

        if (_client == null)
        {
            return;
        }

        _timer?.Dispose();
        await _client?.DisconnectAsync()!;
        _healthMonitor.ClearAllWorlds(_platform);
    }

    public Task OnApplicationStartup(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task OnApplicationShutdown(CancellationToken cancellationToken)
    {
        await _client?.DisconnectAsync()!;
    }

    public Task<object> GetStatusAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((object)_lastStateChange!)!;
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
        catch (Exception)
        {
            _logger.LogError(91097, "Failed to parse message: {0}", message);
            return Task.CompletedTask;
        }

        if (msg.TryGetString("type") == "heartbeat")
        {
            UpdateStateDetails(msg.Deserialize<object>()!);
            return Task.CompletedTask;
        }

#pragma warning disable CS4014
        Task.Run(() =>
        {
            _handler.Process(_platform, msg);
        });
#pragma warning restore CS4014

        return Task.CompletedTask;
    }

    private async Task OnDisconnectAsync(DisconnectionInfo info)
    {
        UpdateStateDetails(info.Exception?.Message ?? info.Type.ToString());
        await _worldMonitor.ClearAllWorldStates(_platform);
        _healthMonitor.ClearAllWorlds(_platform);
    }

    private void UpdateStateDetails(object contents)
    {
        _lastStateChange = new CensusState
        {
            LastStateChange = DateTime.UtcNow,
            Contents = contents
        };
    }

    private async void CheckDataHealthAsync(object? state)
    {
        if (!_state.IsRunning)
        {
            _healthMonitor.ClearAllWorlds(_platform);
            _timer?.Dispose();
            return;
        }

        if (!_healthMonitor.IsHealthy(_platform))
        {
            _logger.LogError(45234, "Census stream for {Platform} has failed health checks. Attempting resetting connection.", _platform);

            try
            {
                await _client?.ReconnectAsync()!;
            }
            catch (Exception ex)
            {
                UpdateStateDetails(ex.Message);

                _logger.LogError(45235, ex, "Failed to reestablish connection to Census for {Platform}", _platform);
            }
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        _timer?.Dispose();
    }
}

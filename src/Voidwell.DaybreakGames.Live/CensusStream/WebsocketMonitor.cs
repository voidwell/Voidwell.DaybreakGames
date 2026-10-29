using System.Text.Json;
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
    private readonly LiveOptions _options;
    private readonly ILogger<WebsocketMonitor> _logger;

    private CensusState? _lastStateChange;

    public WebsocketMonitor(Platform platform, ICensusStreamClient streamClient, IWebsocketEventHandler handler,
        IWorldMonitor worldMonitor, IOptions<LiveOptions> options, ILogger<WebsocketMonitor> logger)
    {
        _platform = platform;
        _client = streamClient;
        _handler = handler;
        _worldMonitor = worldMonitor;
        _options = options.Value;
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
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping census stream monitor for {Platform}", _platform);

        if (_client == null)
        {
            return;
        }

        await _client?.DisconnectAsync()!;
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

        if (msg.TryGetPropertyString("type") == "heartbeat")
        {
            UpdateStateDetails(msg.Deserialize<object>()!);
            return Task.CompletedTask;
        }

        Task.Run(() =>
        {
            _handler.Process(_platform, msg);
        });

        return Task.CompletedTask;
    }

    private async Task OnDisconnectAsync(DisconnectionInfo info)
    {
        UpdateStateDetails(info.Exception?.Message ?? info.Type.ToString());
        await _worldMonitor.ClearAllWorldStates(_platform);
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
        GC.SuppressFinalize(this);
    }
}

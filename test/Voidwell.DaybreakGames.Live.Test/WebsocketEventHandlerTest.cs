using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class WebsocketEventHandlerTest
{
    private readonly Mock<IEventProcessorHandler> _processorHandler = new();
    private readonly Mock<IWorldMonitor> _worldMonitor = new();
    private readonly Mock<IAlertService> _alertService = new();
    private readonly Mock<IMetagameEventService> _metagameEventService = new();
    private readonly Mock<IWebsocketHealthMonitor> _healthMonitor = new();
    private readonly Mock<ILogger<WebsocketEventHandler>> _logger = new();

    private WebsocketEventHandler CreateSut()
    {
        _worldMonitor.Setup(a => a.SetWorldState(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        return new WebsocketEventHandler(_processorHandler.Object, _worldMonitor.Object, _alertService.Object,
            _metagameEventService.Object, _healthMonitor.Object, _logger.Object);
    }

    private static JsonElement Json(string json)
    {
        return JsonDocument.Parse(json).RootElement.Clone();
    }

    [Fact]
    public async Task Process_ServiceStateChangedOnline_SetsWorldOnline()
    {
        _alertService.Setup(a => a.GetActiveAlertsByWorldId(17)).ReturnsAsync(Array.Empty<Alert>());
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceStateChanged","detail":"EventServerEndpoint_Emerald_17","online":"true"}"""));

        _worldMonitor.Verify(a => a.SetWorldState(17, "Emerald", true), Times.Once);
        _healthMonitor.Verify(a => a.ClearWorld(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Process_ServiceStateChangedOnline_RestoresActiveAlerts()
    {
        var start = new DateTime(2024, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        _alertService.Setup(a => a.GetActiveAlertsByWorldId(17)).ReturnsAsync(new[]
        {
            new Alert { WorldId = 17, ZoneId = 2, MetagameEventId = 123, MetagameInstanceId = 55, StartDate = start }
        });
        _metagameEventService.Setup(a => a.GetMetagameEvent(123)).ReturnsAsync(new ZoneMetagameEvent { Id = 123, Duration = TimeSpan.FromMinutes(45) });
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceStateChanged","detail":"EventServerEndpoint_Emerald_17","online":"true"}"""));

        _worldMonitor.Verify(a => a.UpdateZoneAlert(17, 2, It.Is<ZoneAlertState>(s =>
            s.Timestamp == start && s.InstanceId == 55 && s.MetagameEvent.Id == 123)), Times.Once);
    }

    [Fact]
    public async Task Process_ServiceStateChangedOffline_SetsWorldOfflineAndClearsHealthTracking()
    {
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceStateChanged","detail":"EventServerEndpoint_Emerald_17","online":"false"}"""));

        _worldMonitor.Verify(a => a.SetWorldState(17, "Emerald", false), Times.Once);
        _healthMonitor.Verify(a => a.ClearWorld(17), Times.Once);
        _alertService.Verify(a => a.GetActiveAlertsByWorldId(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Process_ServiceStateChangedWithNonNumericWorld_IsIgnored()
    {
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceStateChanged","detail":"EventServerEndpoint_Emerald_notanumber","online":"true"}"""));

        _worldMonitor.Verify(a => a.SetWorldState(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task Process_ServiceStateChangeFailure_IsSwallowed()
    {
        _worldMonitor.Setup(a => a.SetWorldState(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ThrowsAsync(new InvalidOperationException());
        var sut = new WebsocketEventHandler(_processorHandler.Object, _worldMonitor.Object, _alertService.Object,
            _metagameEventService.Object, _healthMonitor.Object, _logger.Object);

        var act = () => sut.Process(Json("""{"type":"serviceStateChanged","detail":"EventServerEndpoint_Emerald_17","online":"true"}"""));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Process_ServiceEvent_RecordsHealthAndDispatchesToProcessor()
    {
        _processorHandler.Setup(a => a.TryProcessAsync("PlayerLogin", It.IsAny<JsonElement>())).ReturnsAsync(true);
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceMessage","payload":{"event_name":"PlayerLogin","world_id":"17","zone_id":"2","timestamp":"1700000000","character_id":"c1"}}"""));

        _healthMonitor.Verify(a => a.ReceivedEvent(17, "PlayerLogin", null), Times.Once);
        _processorHandler.Verify(a => a.TryProcessAsync("PlayerLogin", It.Is<JsonElement>(p => p.GetProperty("character_id").GetString() == "c1")), Times.Once);
    }

    [Fact]
    public async Task Process_ServiceEventInInstancedZone_IsRecordedButNotProcessed()
    {
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceMessage","payload":{"event_name":"Death","world_id":"17","zone_id":"1001","timestamp":"1700000000"}}"""));

        _healthMonitor.Verify(a => a.ReceivedEvent(17, "Death", null), Times.Once);
        _processorHandler.Verify(a => a.TryProcessAsync(It.IsAny<string>(), It.IsAny<JsonElement>()), Times.Never);
    }

    [Fact]
    public async Task Process_ServiceEventWithoutZone_IsProcessed()
    {
        _processorHandler.Setup(a => a.TryProcessAsync("PlayerLogout", It.IsAny<JsonElement>())).ReturnsAsync(true);
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceMessage","payload":{"event_name":"PlayerLogout","world_id":"17","timestamp":"1700000000"}}"""));

        _processorHandler.Verify(a => a.TryProcessAsync("PlayerLogout", It.IsAny<JsonElement>()), Times.Once);
    }

    [Fact]
    public async Task Process_ServiceEventWithoutName_IsNotProcessed()
    {
        var sut = CreateSut();

        await sut.Process(Json("""{"type":"serviceMessage","payload":{"world_id":"17","timestamp":"1700000000"}}"""));

        _processorHandler.Verify(a => a.TryProcessAsync(It.IsAny<string>(), It.IsAny<JsonElement>()), Times.Never);
    }

    [Fact]
    public async Task Process_ServiceEventWithoutProcessor_DoesNotThrow()
    {
        _processorHandler.Setup(a => a.TryProcessAsync("Unknown", It.IsAny<JsonElement>())).ReturnsAsync(false);
        var sut = CreateSut();

        var act = () => sut.Process(Json("""{"type":"serviceMessage","payload":{"event_name":"Unknown","world_id":"17","timestamp":"1700000000"}}"""));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Process_ProcessorFailure_IsSwallowed()
    {
        _processorHandler.Setup(a => a.TryProcessAsync("Death", It.IsAny<JsonElement>())).ThrowsAsync(new InvalidOperationException());
        var sut = CreateSut();

        var act = () => sut.Process(Json("""{"type":"serviceMessage","payload":{"event_name":"Death","world_id":"17","timestamp":"1700000000"}}"""));

        await act.Should().NotThrowAsync();
    }
}

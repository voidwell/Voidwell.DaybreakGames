using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Test;
using Voidwell.DaybreakGames.Test.MapTests;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class WorldMonitorTest : IClassFixture<WorldMonitorFixture>
{
    private readonly WorldMonitorFixture _fixture;

    public WorldMonitorTest(WorldMonitorFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetFixture();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetWorldState_HasWorldState(bool isOnline)
    {
        int worldId = 2;
        int zoneId = 2;
        var testZone = new Zone
        {
            Id = zoneId,
            Name = "Test Zone"
        };
        var mapOwnership = MapHelper.GetMapOwnership(zoneId);
        var zoneMap = MapHelper.GetZoneMap(zoneId);

        _fixture.ZoneStore.AsMock()
            .Setup(a => a.GetPlayableZones())
            .ReturnsAsync(new[] { testZone });

        _fixture.MapService.AsMock()
            .Setup(a => a.GetMapOwnership(worldId, zoneId))
            .ReturnsAsync(mapOwnership);

        _fixture.MapService.AsMock()
            .Setup(a => a.GetZoneMapAsync(zoneId))
            .ReturnsAsync(zoneMap);

        _fixture.MapService.AsMock()
            .Setup(a => a.GetZoneStateHistoricals())
            .ReturnsAsync(new ZoneStateHistorical(Enumerable.Empty<ContinentLock>(), Enumerable.Empty<ContinentUnlock>()));

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(worldId, "test", isOnline);

        var worldState = await sut.GetWorldState(worldId);

        Assert.NotNull(worldState);
        Assert.Equal(worldId, worldState.Id);
        Assert.Equal(isOnline, worldState.IsOnline);
    }

    [Fact]
    public async Task AddAlertToWorldState_AlertInState()
    {
        int worldId = 2;
        int zoneId = 2;
        var testZone = new Zone
        {
            Id = zoneId,
            Name = "Test Zone"
        };
        var mapOwnership = MapHelper.GetMapOwnership(zoneId);
        var zoneMap = MapHelper.GetZoneMap(zoneId);

        var testZoneMetagameEvent = new ZoneMetagameEvent { Id = 1, Description = "my alert", Duration = TimeSpan.FromHours(3) };
        var testZoneAlertState = new ZoneAlertState(DateTime.UtcNow, 1234, testZoneMetagameEvent);

        _fixture.ZoneStore.AsMock()
            .Setup(a => a.GetPlayableZones())
            .ReturnsAsync(new[] { testZone });

        _fixture.MapService.AsMock()
            .Setup(a => a.GetMapOwnership(worldId, zoneId))
            .ReturnsAsync(mapOwnership);

        _fixture.MapService.AsMock()
            .Setup(a => a.GetZoneMapAsync(zoneId))
            .ReturnsAsync(zoneMap);

        _fixture.MapService.AsMock()
            .Setup(a => a.GetZoneStateHistoricals())
            .ReturnsAsync(new ZoneStateHistorical(Enumerable.Empty<ContinentLock>(), Enumerable.Empty<ContinentUnlock>()));

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(worldId, "test", true);

        sut.UpdateZoneAlert(worldId, zoneId, testZoneAlertState);

        var worldState = await sut.GetWorldState(worldId);

        Assert.NotEmpty(worldState.ZoneStates);
        Assert.Equal(zoneId, worldState.ZoneStates.First().Id);
        Assert.True(worldState.ZoneStates.First().IsTracking);
        Assert.Equivalent(testZoneAlertState, worldState.ZoneStates.First().AlertState);
    }

    [Fact]
    public async Task GetWorldStates_ExcludesInactiveWorlds()
    {
        int activeWorldId = 1;
        int legacyWorldId = 2;
        int newWorldId = 3000;

        _fixture.WorldService.AsMock()
            .Setup(a => a.GetAllWorlds())
            .ReturnsAsync(new[]
            {
                new World { Id = activeWorldId, Name = "Osprey" },
                new World { Id = legacyWorldId, Name = "Legacy", IsActive = false }
            });

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(activeWorldId, "Osprey", false);
        await sut.SetWorldState(legacyWorldId, "Legacy", false);
        await sut.SetWorldState(newWorldId, "New", false);

        var worldStates = await sut.GetWorldStates();

        Assert.Contains(worldStates, a => a.Id == activeWorldId);
        Assert.Contains(worldStates, a => a.Id == newWorldId);
        Assert.DoesNotContain(worldStates, a => a.Id == legacyWorldId);
    }
}

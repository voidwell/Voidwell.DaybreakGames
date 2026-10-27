using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Test;
using Voidwell.DaybreakGames.Test.MapTests;
using Voidwell.DaybreakGames.Utils;
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
    public async Task SetWorldState_IgnoresInactiveWorlds()
    {
        int activeWorldId = 1;
        int legacyWorldId = 25;
        int newWorldId = 3000;

        _fixture.WorldService.AsMock()
            .Setup(a => a.GetAllWorlds())
            .ReturnsAsync(new[]
            {
                new World { Id = activeWorldId, Name = "Osprey" },
                new World { Id = legacyWorldId, Name = "Briggs", IsActive = false }
            });

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(activeWorldId, "Osprey", false);
        await sut.SetWorldState(legacyWorldId, "Briggs", true);
        await sut.SetWorldState(newWorldId, "New", false);

        Assert.NotNull(await sut.GetWorldState(activeWorldId));
        Assert.NotNull(await sut.GetWorldState(newWorldId));
        Assert.Null(await sut.GetWorldState(legacyWorldId));
    }

    [Fact]
    public async Task GetWorldStates_ExcludesWorldDeactivatedAfterTracking()
    {
        int activeWorldId = 10;
        int deactivatedWorldId = 13;
        int newWorldId = 3001;

        var worlds = new List<World>
        {
            new World { Id = activeWorldId, Name = "Wainwright" },
            new World { Id = deactivatedWorldId, Name = "Cobalt" }
        };

        _fixture.WorldService.AsMock()
            .Setup(a => a.GetAllWorlds())
            .ReturnsAsync(() => worlds);

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(activeWorldId, "Wainwright", false);
        await sut.SetWorldState(deactivatedWorldId, "Cobalt", false);
        await sut.SetWorldState(newWorldId, "New", false);

        worlds[1].IsActive = false;

        var worldStates = await sut.GetWorldStates();

        Assert.Contains(worldStates, a => a.Id == activeWorldId);
        Assert.Contains(worldStates, a => a.Id == newWorldId);
        Assert.DoesNotContain(worldStates, a => a.Id == deactivatedWorldId);
    }

    [Fact]
    public async Task GetWorldStates_PlatformSet_ReturnsOnlyThatPlatform()
    {
        int pcWorldId = 3010;
        int ps4WorldId = 3011;

        _fixture.WorldService.AsMock()
            .Setup(a => a.GetAllWorlds())
            .ReturnsAsync(new[]
            {
                new World { Id = pcWorldId, Name = "PC world", Platform = Platform.PC },
                new World { Id = ps4WorldId, Name = "PS4 world", Platform = Platform.PS4US }
            });

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(pcWorldId, "PC world", false);
        await sut.SetWorldState(ps4WorldId, "PS4 world", false);

        IEnumerable<WorldOnlineState> worldStates;
        using (_fixture.CensusContext.Use(Platform.PS4US))
        {
            worldStates = await sut.GetWorldStates();
        }

        Assert.Contains(worldStates, a => a.Id == ps4WorldId);
        Assert.DoesNotContain(worldStates, a => a.Id == pcWorldId);
    }

    [Fact]
    public async Task ClearAllWorldStates_OnlyClearsThatPlatform()
    {
        int pcWorldId = 3020;
        int unstoredPs4WorldId = 3021;

        _fixture.WorldService.AsMock()
            .Setup(a => a.GetAllWorlds())
            .ReturnsAsync(new[] { new World { Id = pcWorldId, Name = "PC world", Platform = Platform.PC } });

        var sut = _fixture.CreateSut();

        await sut.SetWorldState(pcWorldId, "PC world", true);
        using (_fixture.CensusContext.Use(Platform.PS4EU))
        {
            // Not stored yet, so it takes the platform of the stream that reported it
            await sut.SetWorldState(unstoredPs4WorldId, "PS4 world", true);
        }

        await sut.ClearAllWorldStates(Platform.PS4EU);

        Assert.True((await sut.GetWorldState(pcWorldId))!.IsOnline);
        Assert.False((await sut.GetWorldState(unstoredPs4WorldId))!.IsOnline);
    }
}

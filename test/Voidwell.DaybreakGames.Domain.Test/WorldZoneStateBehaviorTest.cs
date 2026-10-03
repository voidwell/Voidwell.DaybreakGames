using FluentAssertions;
using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class WorldZoneStateBehaviorTest
{
    private const int Vs = 1;
    private const int Nc = 2;
    private const int Tr = 3;

    // Warpgate(1) - LargeOutpost(2) - AmpStation(3) - Warpgate(4), with BioLab(5) hanging off the amp station
    private static ZoneMap CreateMap()
    {
        return new ZoneMap
        {
            Regions = new[]
            {
                new ZoneRegion { RegionId = 1, FacilityId = 1, FacilityType = "Warpgate" },
                new ZoneRegion { RegionId = 2, FacilityId = 2, FacilityType = "Large Outpost" },
                new ZoneRegion { RegionId = 3, FacilityId = 3, FacilityType = "Amp Station" },
                new ZoneRegion { RegionId = 4, FacilityId = 4, FacilityType = "Warpgate" },
                new ZoneRegion { RegionId = 5, FacilityId = 5, FacilityType = "Bio Lab" }
            },
            Links = new[]
            {
                new ZoneLink { FacilityIdA = 1, FacilityIdB = 2 },
                new ZoneLink { FacilityIdA = 2, FacilityIdB = 3 },
                new ZoneLink { FacilityIdA = 4, FacilityIdB = 3 },
                new ZoneLink { FacilityIdA = 3, FacilityIdB = 5 }
            }
        };
    }

    private static IEnumerable<ZoneRegionOwnership> CreateOwnership()
    {
        return new[]
        {
            new ZoneRegionOwnership(1, Vs),
            new ZoneRegionOwnership(2, Vs),
            new ZoneRegionOwnership(3, Nc),
            new ZoneRegionOwnership(4, Nc),
            new ZoneRegionOwnership(5, 0)
        };
    }

    private static WorldZoneState CreateTrackedState()
    {
        return new WorldZoneState(10, 2, "Indar", CreateMap(), CreateOwnership());
    }

    [Fact]
    public void Constructor_WithoutMap_IsNotTracking()
    {
        var sut = new WorldZoneState(10, 2, "Indar");

        sut.IsTracking.Should().BeFalse();
        sut.Map.Should().BeNull();
        sut.GetMapOwnership().Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMap_IsTrackingAndBuildsLinkedRegions()
    {
        var sut = CreateTrackedState();

        sut.IsTracking.Should().BeTrue();
        sut.Map.Regions.Should().HaveCount(5);
        sut.Map.Warpgates.Select(a => a.RegionId).Should().Equal(1, 4);
        sut.GetRegionByFacilityId(3).Links.Select(a => a.RegionId).Should().BeEquivalentTo(new[] { 2, 4, 5 });
    }

    [Fact]
    public void MapScore_CountsTerritoriesAndFacilityTypesPerFaction()
    {
        var score = CreateTrackedState().MapScore;

        score.Territories.Vs.Value.Should().Be(2);
        score.Territories.Nc.Value.Should().Be(2);
        score.Territories.Neutural.Value.Should().Be(1);
        score.LargeOutposts.Vs.Value.Should().Be(1);
        score.AmpStations.Nc.Value.Should().Be(1);
        score.BioLabs.Neutural.Value.Should().Be(1);
        score.TechPlants.Vs.Value.Should().Be(0);
    }

    [Fact]
    public void MapScore_ConnectedTerritories_CountsRegionsReachableFromWarpgate()
    {
        var score = CreateTrackedState().MapScore;

        score.ConnectedTerritories.Vs.Value.Should().Be(2);
        score.ConnectedTerritories.Nc.Value.Should().Be(2);
        score.ConnectedTerritories.Tr.Value.Should().Be(0);
    }

    [Fact]
    public async Task FacilityFactionChangeAsync_UpdatesOwnershipAndScore()
    {
        var sut = CreateTrackedState();

        await sut.FacilityFactionChangeAsync(facilityId: 3, factionId: Vs);

        sut.GetMapOwnership().Single(a => a.RegionId == 3).FactionId.Should().Be(Vs);
        sut.MapScore.Territories.Vs.Value.Should().Be(3);
        sut.MapScore.Territories.Nc.Value.Should().Be(1);
        sut.MapScore.ConnectedTerritories.Vs.Value.Should().Be(3);
    }

    [Fact]
    public async Task FacilityFactionChangeAsync_UnknownFacility_ChangesNothing()
    {
        var sut = CreateTrackedState();

        await sut.FacilityFactionChangeAsync(facilityId: 999, factionId: Tr);

        sut.MapScore.Territories.Tr.Value.Should().Be(0);
    }

    [Fact]
    public void UpdateLockState_Locked_ClearsAlertState()
    {
        var sut = CreateTrackedState();
        sut.UpdateAlertState(new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent { Duration = TimeSpan.FromHours(1) }));

        sut.UpdateLockState(new ZoneLockState(DateTime.UtcNow, 1, Vs));

        sut.GetAlertState().Should().BeNull();
        sut.LockState.State.Should().Be(ZoneLockStateEnum.LOCKED);
    }

    [Fact]
    public void UpdateLockState_Unlocked_KeepsAlertState()
    {
        var sut = CreateTrackedState();
        var alert = new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent { Duration = TimeSpan.FromHours(1) });
        sut.UpdateAlertState(alert);

        sut.UpdateLockState(new ZoneLockState(DateTime.UtcNow));

        sut.GetAlertState().Should().BeSameAs(alert);
    }

    [Fact]
    public void GetAlertState_EndedAlert_IsClearedAndReturnsNull()
    {
        var sut = CreateTrackedState();
        sut.UpdateAlertState(new ZoneAlertState(DateTime.UtcNow.AddHours(-2), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(90) }));

        sut.GetAlertState().Should().BeNull();
    }

    [Fact]
    public void GetAlertState_ActiveAlert_IsReturned()
    {
        var sut = CreateTrackedState();
        var alert = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-10), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(90) });
        sut.UpdateAlertState(alert);

        sut.GetAlertState().Should().BeSameAs(alert);
    }

    [Fact]
    public void DisableTracking_ClearsLockAlertAndTracking()
    {
        var sut = CreateTrackedState();
        sut.UpdateLockState(new ZoneLockState(DateTime.UtcNow, 1, Vs));
        sut.UpdateAlertState(new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent()));

        sut.DisableTracking();

        sut.IsTracking.Should().BeFalse();
        sut.LockState.Should().BeNull();
        sut.GetAlertState().Should().BeNull();
        sut.GetMapOwnership().Should().BeNull();
    }
}

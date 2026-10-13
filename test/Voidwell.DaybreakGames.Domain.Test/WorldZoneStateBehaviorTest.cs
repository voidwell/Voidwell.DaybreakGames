using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class WorldZoneStateBehaviorTest
{
    private const int _vs = 1;
    private const int _nc = 2;
    private const int _tr = 3;

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
            new ZoneRegionOwnership(1, _vs),
            new ZoneRegionOwnership(2, _vs),
            new ZoneRegionOwnership(3, _nc),
            new ZoneRegionOwnership(4, _nc),
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

        Assert.False(sut.IsTracking);
        Assert.Null(sut.Map);
        Assert.Null(sut.GetMapOwnership());
    }

    [Fact]
    public void Constructor_WithMap_IsTrackingAndBuildsLinkedRegions()
    {
        var sut = CreateTrackedState();

        Assert.True(sut.IsTracking);
        Assert.Equal(5, sut.Map.Regions.Count());
        Assert.Equal(new[] { 1, 4 }, sut.Map.Warpgates.Select(a => a.RegionId));
        Assert.Equivalent(new[] { 2, 4, 5 }, sut.GetRegionByFacilityId(3).Links.Select(a => a.RegionId));
    }

    [Fact]
    public void MapScore_CountsTerritoriesAndFacilityTypesPerFaction()
    {
        var score = CreateTrackedState().MapScore;

        Assert.Equal(2, score.Territories.Vs.Value);
        Assert.Equal(2, score.Territories.Nc.Value);
        Assert.Equal(1, score.Territories.Neutural.Value);
        Assert.Equal(1, score.LargeOutposts.Vs.Value);
        Assert.Equal(1, score.AmpStations.Nc.Value);
        Assert.Equal(1, score.BioLabs.Neutural.Value);
        Assert.Equal(0, score.TechPlants.Vs.Value);
    }

    [Fact]
    public void MapScore_ConnectedTerritories_CountsRegionsReachableFromWarpgate()
    {
        var score = CreateTrackedState().MapScore;

        Assert.Equal(2, score.ConnectedTerritories.Vs.Value);
        Assert.Equal(2, score.ConnectedTerritories.Nc.Value);
        Assert.Equal(0, score.ConnectedTerritories.Tr.Value);
    }

    [Fact]
    public async Task FacilityFactionChangeAsync_UpdatesOwnershipAndScore()
    {
        var sut = CreateTrackedState();

        await sut.FacilityFactionChangeAsync(facilityId: 3, factionId: _vs);

        Assert.Equal(_vs, sut.GetMapOwnership().Single(a => a.RegionId == 3).FactionId);
        Assert.Equal(3, sut.MapScore.Territories.Vs.Value);
        Assert.Equal(1, sut.MapScore.Territories.Nc.Value);
        Assert.Equal(3, sut.MapScore.ConnectedTerritories.Vs.Value);
    }

    [Fact]
    public async Task FacilityFactionChangeAsync_UnknownFacility_ChangesNothing()
    {
        var sut = CreateTrackedState();

        await sut.FacilityFactionChangeAsync(facilityId: 999, factionId: _tr);

        Assert.Equal(0, sut.MapScore.Territories.Tr.Value);
    }

    [Fact]
    public void UpdateLockState_Locked_ClearsAlertState()
    {
        var sut = CreateTrackedState();
        sut.UpdateAlertState(new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent { Duration = TimeSpan.FromHours(1) }));

        sut.UpdateLockState(new ZoneLockState(DateTime.UtcNow, 1, _vs));

        Assert.Null(sut.GetAlertState());
        Assert.Equal(ZoneLockStateEnum.LOCKED, sut.LockState.State);
    }

    [Fact]
    public void UpdateLockState_Unlocked_KeepsAlertState()
    {
        var sut = CreateTrackedState();
        var alert = new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent { Duration = TimeSpan.FromHours(1) });
        sut.UpdateAlertState(alert);

        sut.UpdateLockState(new ZoneLockState(DateTime.UtcNow));

        Assert.Same(alert, sut.GetAlertState());
    }

    [Fact]
    public void GetAlertState_EndedAlert_IsClearedAndReturnsNull()
    {
        var sut = CreateTrackedState();
        sut.UpdateAlertState(new ZoneAlertState(DateTime.UtcNow.AddHours(-2), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(90) }));

        Assert.Null(sut.GetAlertState());
    }

    [Fact]
    public void GetAlertState_ActiveAlert_IsReturned()
    {
        var sut = CreateTrackedState();
        var alert = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-10), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(90) });
        sut.UpdateAlertState(alert);

        Assert.Same(alert, sut.GetAlertState());
    }

    [Fact]
    public void DisableTracking_ClearsLockAlertAndTracking()
    {
        var sut = CreateTrackedState();
        sut.UpdateLockState(new ZoneLockState(DateTime.UtcNow, 1, _vs));
        sut.UpdateAlertState(new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent()));

        sut.DisableTracking();

        Assert.False(sut.IsTracking);
        Assert.Null(sut.LockState);
        Assert.Null(sut.GetAlertState());
        Assert.Null(sut.GetMapOwnership());
    }
}

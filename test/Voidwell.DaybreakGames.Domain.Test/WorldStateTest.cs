using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class WorldStateTest
{
    private static ZoneMap CreateMap()
    {
        return new ZoneMap
        {
            Regions = new[]
            {
                new ZoneRegion { RegionId = 1, FacilityId = 1, FacilityType = "Warpgate" },
                new ZoneRegion { RegionId = 2, FacilityId = 2, FacilityType = "Small Outpost" }
            },
            Links = new[] { new ZoneLink { FacilityIdA = 1, FacilityIdB = 2 } }
        };
    }

    private static ZoneRegionOwnership[] CreateOwnership()
    {
        return new[] { new ZoneRegionOwnership(1, 1), new ZoneRegionOwnership(2, 1) };
    }

    [Fact]
    public void Constructor_StartsOfflineWithNoZones()
    {
        var sut = new WorldState(17, "Emerald");

        Assert.Equal(17, sut.Id);
        Assert.Equal("Emerald", sut.Name);
        Assert.False(sut.IsOnline);
        Assert.Empty(sut.GetZoneStates());
    }

    [Fact]
    public void SetWorldOnline_MarksOnlineAndClearsZones()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        sut.SetWorldOnline();

        Assert.True(sut.IsOnline);
        Assert.Empty(sut.GetZoneStates());
    }

    [Fact]
    public void InitZoneState_AddsUntrackedZone()
    {
        var sut = new WorldState(17, "Emerald");

        sut.InitZoneState(2, "Indar");

        var zone = sut.GetZoneState(2);
        Assert.Equal(2, zone.Id);
        Assert.Equal("Indar", zone.Name);
        Assert.False(zone.IsTracking);
    }

    [Fact]
    public void GetZoneState_UnknownZone_ReturnsNull()
    {
        Assert.Null(new WorldState(17, "Emerald").GetZoneState(2));
    }

    [Fact]
    public void SetZoneState_ExistingZone_ReplacesIt()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        sut.SetZoneState(new WorldZoneState(17, 2, "Renamed"));

        Assert.Equal("Renamed", Assert.Single(sut.GetZoneStates()).Name);
    }

    [Fact]
    public void TrySetupZoneState_UnknownZone_ReturnsFalse()
    {
        var sut = new WorldState(17, "Emerald");

        Assert.False(sut.TrySetupZoneState(2, CreateMap(), CreateOwnership()));
    }

    [Fact]
    public void TrySetupZoneState_KnownZone_StartsTracking()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        Assert.True(sut.TrySetupZoneState(2, CreateMap(), CreateOwnership()));

        Assert.True(sut.GetZoneState(2).IsTracking);
        Assert.Equal(2, sut.GetZoneMapOwnership(2).Count());
        Assert.Equal(2, sut.GetZoneMapScore(2).Territories.Vs.Value);
    }

    [Fact]
    public void ZoneMapQueries_UntrackedOrUnknownZone_ReturnNull()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        Assert.Null(sut.GetZoneMapOwnership(2));
        Assert.Null(sut.GetZoneMapScore(2));
        Assert.Null(sut.GetZoneMapOwnership(99));
        Assert.Null(sut.GetZoneMapScore(99));
    }

    [Fact]
    public void UpdateZoneLockState_SetsLockStateOnZone()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");
        var lockState = new ZoneLockState(DateTime.UtcNow, 5, 1);

        sut.UpdateZoneLockState(2, lockState);

        Assert.Same(lockState, sut.GetZoneState(2).LockState);
    }

    [Fact]
    public void UpdateZoneLockStateAndAlertState_UnknownZone_AreIgnored()
    {
        var sut = new WorldState(17, "Emerald");

        var act = () =>
        {
            sut.UpdateZoneLockState(99, new ZoneLockState(DateTime.UtcNow));
            sut.UpdateZoneAlertState(99);
        };

        Assert.Null(Record.Exception(act));
    }

    [Fact]
    public void UpdateZoneAlertState_SetsAlertOnZone()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");
        var alert = new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent { Duration = TimeSpan.FromHours(1) });

        sut.UpdateZoneAlertState(2, alert);

        Assert.Same(alert, sut.GetZoneState(2).AlertState);
    }

    [Fact]
    public void SetWorldOffline_StopsTrackingAllZones()
    {
        var sut = new WorldState(17, "Emerald");
        sut.SetWorldOnline();
        sut.InitZoneState(2, "Indar");
        sut.TrySetupZoneState(2, CreateMap(), CreateOwnership());

        sut.SetWorldOffline();

        Assert.False(sut.IsOnline);
        Assert.False(sut.GetZoneState(2).IsTracking);
    }

    [Fact]
    public async Task UpdateZoneFacilityFactionAsync_TrackedZone_ReturnsRegionAndScore()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");
        sut.TrySetupZoneState(2, CreateMap(), CreateOwnership());

        var change = await sut.UpdateZoneFacilityFactionAsync(2, facilityId: 2, factionId: 3);

        Assert.Equal(2, change.Region.FacilityId);
        Assert.Equal(1, change.Score.Territories.Tr.Value);
        Assert.Equal(1, change.Score.Territories.Vs.Value);
    }

    [Fact]
    public async Task UpdateZoneFacilityFactionAsync_UntrackedZone_ReturnsNull()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        Assert.Null((await sut.UpdateZoneFacilityFactionAsync(2, 2, 3)));
        Assert.Null((await sut.UpdateZoneFacilityFactionAsync(99, 2, 3)));
    }
}

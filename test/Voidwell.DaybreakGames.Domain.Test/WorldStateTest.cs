using FluentAssertions;
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

        sut.Id.Should().Be(17);
        sut.Name.Should().Be("Emerald");
        sut.IsOnline.Should().BeFalse();
        sut.GetZoneStates().Should().BeEmpty();
    }

    [Fact]
    public void SetWorldOnline_MarksOnlineAndClearsZones()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        sut.SetWorldOnline();

        sut.IsOnline.Should().BeTrue();
        sut.GetZoneStates().Should().BeEmpty();
    }

    [Fact]
    public void InitZoneState_AddsUntrackedZone()
    {
        var sut = new WorldState(17, "Emerald");

        sut.InitZoneState(2, "Indar");

        var zone = sut.GetZoneState(2);
        zone.Id.Should().Be(2);
        zone.Name.Should().Be("Indar");
        zone.IsTracking.Should().BeFalse();
    }

    [Fact]
    public void GetZoneState_UnknownZone_ReturnsNull()
    {
        new WorldState(17, "Emerald").GetZoneState(2).Should().BeNull();
    }

    [Fact]
    public void SetZoneState_ExistingZone_ReplacesIt()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        sut.SetZoneState(new WorldZoneState(17, 2, "Renamed"));

        sut.GetZoneStates().Should().ContainSingle().Which.Name.Should().Be("Renamed");
    }

    [Fact]
    public void TrySetupZoneState_UnknownZone_ReturnsFalse()
    {
        var sut = new WorldState(17, "Emerald");

        sut.TrySetupZoneState(2, CreateMap(), CreateOwnership()).Should().BeFalse();
    }

    [Fact]
    public void TrySetupZoneState_KnownZone_StartsTracking()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        sut.TrySetupZoneState(2, CreateMap(), CreateOwnership()).Should().BeTrue();

        sut.GetZoneState(2).IsTracking.Should().BeTrue();
        sut.GetZoneMapOwnership(2).Should().HaveCount(2);
        sut.GetZoneMapScore(2).Territories.Vs.Value.Should().Be(2);
    }

    [Fact]
    public void ZoneMapQueries_UntrackedOrUnknownZone_ReturnNull()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        sut.GetZoneMapOwnership(2).Should().BeNull();
        sut.GetZoneMapScore(2).Should().BeNull();
        sut.GetZoneMapOwnership(99).Should().BeNull();
        sut.GetZoneMapScore(99).Should().BeNull();
    }

    [Fact]
    public void UpdateZoneLockState_SetsLockStateOnZone()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");
        var lockState = new ZoneLockState(DateTime.UtcNow, 5, 1);

        sut.UpdateZoneLockState(2, lockState);

        sut.GetZoneState(2).LockState.Should().BeSameAs(lockState);
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

        act.Should().NotThrow();
    }

    [Fact]
    public void UpdateZoneAlertState_SetsAlertOnZone()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");
        var alert = new ZoneAlertState(DateTime.UtcNow, 1, new ZoneMetagameEvent { Duration = TimeSpan.FromHours(1) });

        sut.UpdateZoneAlertState(2, alert);

        sut.GetZoneState(2).AlertState.Should().BeSameAs(alert);
    }

    [Fact]
    public void SetWorldOffline_StopsTrackingAllZones()
    {
        var sut = new WorldState(17, "Emerald");
        sut.SetWorldOnline();
        sut.InitZoneState(2, "Indar");
        sut.TrySetupZoneState(2, CreateMap(), CreateOwnership());

        sut.SetWorldOffline();

        sut.IsOnline.Should().BeFalse();
        sut.GetZoneState(2).IsTracking.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateZoneFacilityFactionAsync_TrackedZone_ReturnsRegionAndScore()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");
        sut.TrySetupZoneState(2, CreateMap(), CreateOwnership());

        var change = await sut.UpdateZoneFacilityFactionAsync(2, facilityId: 2, factionId: 3);

        change.Region.FacilityId.Should().Be(2);
        change.Score.Territories.Tr.Value.Should().Be(1);
        change.Score.Territories.Vs.Value.Should().Be(1);
    }

    [Fact]
    public async Task UpdateZoneFacilityFactionAsync_UntrackedZone_ReturnsNull()
    {
        var sut = new WorldState(17, "Emerald");
        sut.InitZoneState(2, "Indar");

        (await sut.UpdateZoneFacilityFactionAsync(2, 2, 3)).Should().BeNull();
        (await sut.UpdateZoneFacilityFactionAsync(99, 2, 3)).Should().BeNull();
    }
}

using System.Text.Json;
using FluentAssertions;
using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class ZoneStateModelsTest
{
    [Fact]
    public void ZoneLockState_LockedConstructor_SetsDetails()
    {
        var timestamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var sut = new ZoneLockState(timestamp, 5, 2);

        sut.State.Should().Be(ZoneLockStateEnum.LOCKED);
        sut.Timestamp.Should().Be(timestamp);
        sut.MetagameEventId.Should().Be(5);
        sut.TriggeringFaction.Should().Be(2);
    }

    [Fact]
    public void ZoneLockState_UnlockedConstructor_HasNoDetails()
    {
        var sut = new ZoneLockState(DateTime.UtcNow);

        sut.State.Should().Be(ZoneLockStateEnum.UNLOCKED);
        sut.MetagameEventId.Should().BeNull();
        sut.TriggeringFaction.Should().BeNull();
    }

    [Fact]
    public void ZoneLockState_SerializesStateAsString()
    {
        var json = JsonSerializer.Serialize(new ZoneLockState(DateTime.UtcNow, 1, 2));

        json.Should().Contain("\"State\":\"LOCKED\"");
    }

    [Fact]
    public void ZoneAlertState_IsEventEnded_UsesMetagameEventDuration()
    {
        var running = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-20), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(30) });
        var ended = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-40), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(30) });

        running.IsEventEnded().Should().BeFalse();
        ended.IsEventEnded().Should().BeTrue();
    }

    [Fact]
    public void ZoneAlertState_IsEventEnded_DefaultsToNinetyMinutes()
    {
        var running = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-80), 1, new ZoneMetagameEvent());
        var ended = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-100), 1, new ZoneMetagameEvent());
        var noEvent = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-100), 1, null);

        running.IsEventEnded().Should().BeFalse();
        ended.IsEventEnded().Should().BeTrue();
        noEvent.IsEventEnded().Should().BeTrue();
    }

    [Fact]
    public void WorldZone_BuildsRegionsWarpgatesAndBidirectionalLinks()
    {
        var map = new ZoneMap
        {
            Regions = new[]
            {
                new ZoneRegion { RegionId = 10, FacilityId = 100, FacilityType = "Warpgate" },
                new ZoneRegion { RegionId = 11, FacilityId = 101, FacilityType = "Small Outpost" }
            },
            Links = new[]
            {
                new ZoneLink { FacilityIdA = 100, FacilityIdB = 101 },
                new ZoneLink { FacilityIdA = 100, FacilityIdB = 999 } // facility missing from the map
            }
        };

        var sut = new WorldZone(map);

        sut.Regions.Should().HaveCount(2);
        sut.Warpgates.Should().ContainSingle().Which.RegionId.Should().Be(10);
        sut.Regions[0].Links.Should().ContainSingle().Which.RegionId.Should().Be(11);
        sut.Regions[1].Links.Should().ContainSingle().Which.RegionId.Should().Be(10);
    }
}

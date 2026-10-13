using System.Text.Json;
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

        Assert.Equal(ZoneLockStateEnum.LOCKED, sut.State);
        Assert.Equal(timestamp, sut.Timestamp);
        Assert.Equal(5, sut.MetagameEventId);
        Assert.Equal(2, sut.TriggeringFaction);
    }

    [Fact]
    public void ZoneLockState_UnlockedConstructor_HasNoDetails()
    {
        var sut = new ZoneLockState(DateTime.UtcNow);

        Assert.Equal(ZoneLockStateEnum.UNLOCKED, sut.State);
        Assert.Null(sut.MetagameEventId);
        Assert.Null(sut.TriggeringFaction);
    }

    [Fact]
    public void ZoneLockState_SerializesStateAsString()
    {
        var json = JsonSerializer.Serialize(new ZoneLockState(DateTime.UtcNow, 1, 2));

        Assert.Contains("\"State\":\"LOCKED\"", json);
    }

    [Fact]
    public void ZoneAlertState_IsEventEnded_UsesMetagameEventDuration()
    {
        var running = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-20), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(30) });
        var ended = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-40), 1, new ZoneMetagameEvent { Duration = TimeSpan.FromMinutes(30) });

        Assert.False(running.IsEventEnded());
        Assert.True(ended.IsEventEnded());
    }

    [Fact]
    public void ZoneAlertState_IsEventEnded_DefaultsToNinetyMinutes()
    {
        var running = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-80), 1, new ZoneMetagameEvent());
        var ended = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-100), 1, new ZoneMetagameEvent());
        var noEvent = new ZoneAlertState(DateTime.UtcNow.AddMinutes(-100), 1, null);

        Assert.False(running.IsEventEnded());
        Assert.True(ended.IsEventEnded());
        Assert.True(noEvent.IsEventEnded());
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

        Assert.Equal(2, sut.Regions.Count());
        Assert.Equal(10, Assert.Single(sut.Warpgates).RegionId);
        Assert.Equal(11, Assert.Single(sut.Regions[0].Links).RegionId);
        Assert.Equal(10, Assert.Single(sut.Regions[1].Links).RegionId);
    }
}

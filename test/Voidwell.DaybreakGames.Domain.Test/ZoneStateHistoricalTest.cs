using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class ZoneStateHistoricalTest
{
    private static readonly DateTime _now = new(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private static ContinentLock Lock(int worldId, int zoneId, DateTime timestamp, int? metagameEventId = null, int? faction = null)
    {
        return new ContinentLock { WorldId = worldId, ZoneId = zoneId, Timestamp = timestamp, MetagameEventId = metagameEventId, TriggeringFaction = faction };
    }

    private static ContinentUnlock Unlock(int worldId, int zoneId, DateTime timestamp)
    {
        return new ContinentUnlock { WorldId = worldId, ZoneId = zoneId, Timestamp = timestamp };
    }

    [Fact]
    public void GetLastLockState_NoEvents_ReturnsNull()
    {
        var sut = new ZoneStateHistorical(Array.Empty<ContinentLock>(), Array.Empty<ContinentUnlock>());

        Assert.Null(sut.GetLastLockState(1, 2));
    }

    [Fact]
    public void GetLastLockState_OnlyLock_ReturnsLockedStateWithDetails()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, _now, metagameEventId: 7, faction: 3) }, Array.Empty<ContinentUnlock>());

        var state = sut.GetLastLockState(1, 2);

        Assert.Equal(ZoneLockStateEnum.LOCKED, state.State);
        Assert.Equal(_now, state.Timestamp);
        Assert.Equal(7, state.MetagameEventId);
        Assert.Equal(3, state.TriggeringFaction);
    }

    [Fact]
    public void GetLastLockState_OnlyUnlock_ReturnsUnlockedState()
    {
        var sut = new ZoneStateHistorical(Array.Empty<ContinentLock>(), new[] { Unlock(1, 2, _now) });

        var state = sut.GetLastLockState(1, 2);

        Assert.Equal(ZoneLockStateEnum.UNLOCKED, state.State);
        Assert.Equal(_now, state.Timestamp);
    }

    [Fact]
    public void GetLastLockState_LockNewerThanUnlock_ReturnsLocked()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, _now) }, new[] { Unlock(1, 2, _now.AddHours(-1)) });

        Assert.Equal(ZoneLockStateEnum.LOCKED, sut.GetLastLockState(1, 2).State);
    }

    [Fact]
    public void GetLastLockState_UnlockNewerThanLock_ReturnsUnlocked()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, _now.AddHours(-1)) }, new[] { Unlock(1, 2, _now) });

        var state = sut.GetLastLockState(1, 2);

        Assert.Equal(ZoneLockStateEnum.UNLOCKED, state.State);
        Assert.Equal(_now, state.Timestamp);
    }

    [Fact]
    public void GetLastLockState_LockAndUnlockAtSameTime_ReturnsNull()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, _now) }, new[] { Unlock(1, 2, _now) });

        Assert.Null(sut.GetLastLockState(1, 2));
    }

    [Fact]
    public void GetLastLockState_IsScopedPerWorldAndZone()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, _now) }, Array.Empty<ContinentUnlock>());

        Assert.Null(sut.GetLastLockState(1, 3));
        Assert.Null(sut.GetLastLockState(2, 2));
        Assert.NotNull(sut.GetLastLock(1, 2));
        Assert.Null(sut.GetLastUnlock(1, 2));
    }

    [Fact]
    public void Constructor_DuplicateLockForSameWorldAndZone_Throws()
    {
        var act = () => new ZoneStateHistorical(new[] { Lock(1, 2, _now), Lock(1, 2, _now.AddMinutes(1)) }, Array.Empty<ContinentUnlock>());

        Assert.ThrowsAny<ArgumentException>(act);
    }
}

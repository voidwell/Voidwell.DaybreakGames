using FluentAssertions;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class ZoneStateHistoricalTest
{
    private static readonly DateTime Now = new(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);

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

        sut.GetLastLockState(1, 2).Should().BeNull();
    }

    [Fact]
    public void GetLastLockState_OnlyLock_ReturnsLockedStateWithDetails()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, Now, metagameEventId: 7, faction: 3) }, Array.Empty<ContinentUnlock>());

        var state = sut.GetLastLockState(1, 2);

        state.State.Should().Be(ZoneLockStateEnum.LOCKED);
        state.Timestamp.Should().Be(Now);
        state.MetagameEventId.Should().Be(7);
        state.TriggeringFaction.Should().Be(3);
    }

    [Fact]
    public void GetLastLockState_OnlyUnlock_ReturnsUnlockedState()
    {
        var sut = new ZoneStateHistorical(Array.Empty<ContinentLock>(), new[] { Unlock(1, 2, Now) });

        var state = sut.GetLastLockState(1, 2);

        state.State.Should().Be(ZoneLockStateEnum.UNLOCKED);
        state.Timestamp.Should().Be(Now);
    }

    [Fact]
    public void GetLastLockState_LockNewerThanUnlock_ReturnsLocked()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, Now) }, new[] { Unlock(1, 2, Now.AddHours(-1)) });

        sut.GetLastLockState(1, 2).State.Should().Be(ZoneLockStateEnum.LOCKED);
    }

    [Fact]
    public void GetLastLockState_UnlockNewerThanLock_ReturnsUnlocked()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, Now.AddHours(-1)) }, new[] { Unlock(1, 2, Now) });

        var state = sut.GetLastLockState(1, 2);

        state.State.Should().Be(ZoneLockStateEnum.UNLOCKED);
        state.Timestamp.Should().Be(Now);
    }

    [Fact]
    public void GetLastLockState_LockAndUnlockAtSameTime_ReturnsNull()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, Now) }, new[] { Unlock(1, 2, Now) });

        sut.GetLastLockState(1, 2).Should().BeNull();
    }

    [Fact]
    public void GetLastLockState_IsScopedPerWorldAndZone()
    {
        var sut = new ZoneStateHistorical(new[] { Lock(1, 2, Now) }, Array.Empty<ContinentUnlock>());

        sut.GetLastLockState(1, 3).Should().BeNull();
        sut.GetLastLockState(2, 2).Should().BeNull();
        sut.GetLastLock(1, 2).Should().NotBeNull();
        sut.GetLastUnlock(1, 2).Should().BeNull();
    }

    [Fact]
    public void Constructor_DuplicateLockForSameWorldAndZone_Throws()
    {
        var act = () => new ZoneStateHistorical(new[] { Lock(1, 2, Now), Lock(1, 2, Now.AddMinutes(1)) }, Array.Empty<ContinentUnlock>());

        act.Should().Throw<ArgumentException>();
    }
}

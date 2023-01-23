using System.Collections.Generic;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class ZoneStateHistorical
    {
        private readonly Dictionary<int, Dictionary<int, ContinentLock>> _zoneLockEvents = new Dictionary<int, Dictionary<int, ContinentLock>>();
        private readonly Dictionary<int, Dictionary<int, ContinentUnlock>> _zoneUnlockEvents = new Dictionary<int, Dictionary<int, ContinentUnlock>>();

        public ZoneStateHistorical(IEnumerable<ContinentLock> zoneLockEvents, IEnumerable<ContinentUnlock> zoneUnlockEvents)
        {
            foreach (var lockEvent in zoneLockEvents)
            {
                if (!_zoneLockEvents.ContainsKey(lockEvent.WorldId))
                {
                    _zoneLockEvents[lockEvent.WorldId] = new Dictionary<int, ContinentLock>();
                }

                _zoneLockEvents[lockEvent.WorldId].Add(lockEvent.ZoneId, lockEvent);
            }

            foreach (var unlockEvent in zoneUnlockEvents)
            {
                if (!_zoneUnlockEvents.ContainsKey(unlockEvent.WorldId))
                {
                    _zoneUnlockEvents[unlockEvent.WorldId] = new Dictionary<int, ContinentUnlock>();
                }

                _zoneUnlockEvents[unlockEvent.WorldId].Add(unlockEvent.ZoneId, unlockEvent);
            }
        }

        public ContinentUnlock GetLastUnlock(int worldId, int zoneId)
        {
            if (!_zoneUnlockEvents.ContainsKey(worldId) || !_zoneUnlockEvents[worldId].ContainsKey(zoneId))
            {
                return null;
            }

            return _zoneUnlockEvents[worldId][zoneId];
        }

        public ContinentLock GetLastLock(int worldId, int zoneId)
        {
            if (!_zoneLockEvents.ContainsKey(worldId) || !_zoneLockEvents[worldId].ContainsKey(zoneId))
            {
                return null;
            }

            return _zoneLockEvents[worldId][zoneId];
        }

        public ZoneLockState GetLastLockState(int worldId, int zoneId)
        {
            var lastLock = GetLastLock(worldId, zoneId);
            var lastUnlock = GetLastUnlock(worldId, zoneId);
            
            if (lastLock != null && lastUnlock == null || lastLock != null && lastLock.Timestamp > lastUnlock.Timestamp)
            {
                return new ZoneLockState(lastLock.Timestamp, lastLock.MetagameEventId, lastLock.TriggeringFaction);
            }

            if (lastLock == null && lastUnlock != null || lastLock != null && lastLock.Timestamp < lastUnlock.Timestamp)
            {
                return new ZoneLockState(lastUnlock.Timestamp);
            }

            return null;
        }
    }
}

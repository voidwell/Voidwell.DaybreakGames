using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldState
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public bool IsOnline { get; private set; } = false;

        private ConcurrentDictionary<int, WorldZoneState> ZoneStates { get; set; }
        private static readonly TimeSpan MaximumIdleDuration = TimeSpan.FromMinutes(10);

        public WorldState(int worldId, string worldName)
        {
            Id = worldId;
            Name = worldName;
            ZoneStates = new ConcurrentDictionary<int, WorldZoneState>();
        }

        public void SetWorldOnline()
        {
            ZoneStates.Clear();
            IsOnline = true;
        }

        public void SetWorldOffline()
        {
            foreach(var zone in ZoneStates)
            {
                zone.Value.DisableTracking();
            }
            IsOnline = false;
        }

        public void SetZoneState(WorldZoneState zoneState)
        {
            if (!ZoneStates.ContainsKey(zoneState.ZoneId))
            {
                ZoneStates.TryAdd(zoneState.ZoneId, zoneState);
                return;
            }

            ZoneStates[zoneState.ZoneId] = zoneState;
        }

        public void InitZoneState(Zone zone)
        {
            var zoneState = new WorldZoneState(Id, zone);
            SetZoneState(zoneState);
        }

        public bool TrySetupZoneState(int zoneId, ZoneMap zoneMap, IEnumerable<ZoneRegionOwnership> ownership)
        {
            if (!ZoneStates.ContainsKey(zoneId))
            {
                return false;
            }

            ZoneStates[zoneId].Setup(zoneMap, ownership);
            return true;
        }

        public IEnumerable<WorldOnlineZoneState> GetZoneStates()
        {
            return ZoneStates.Keys.Select(GetZoneState);
        }

        public WorldOnlineZoneState GetZoneState(int zoneId)
        {
            if (!ZoneStates.ContainsKey(zoneId))
            {
                return null;
            }

            var zoneState = ZoneStates[zoneId];
            return new WorldOnlineZoneState
            {
                Id = zoneId,
                Name = zoneState.Name,
                IsTracking = zoneState.IsTracking,
                LockState = zoneState.LockState,
                AlertState = zoneState.AlertState
            };
        }
        
        public IEnumerable<ZoneRegionOwnership> GetZoneMapOwnership(int zoneId)
        {
            if (!ZoneStates.ContainsKey(zoneId) || !ZoneStates[zoneId].IsTracking)
            {
                return null;
            }

            return ZoneStates[zoneId].GetMapOwnership() ?? Enumerable.Empty<ZoneRegionOwnership>();
        }

        public MapScore GetZoneMapScore(int zoneId)
        {
            if (!ZoneStates.ContainsKey(zoneId) || !ZoneStates[zoneId].IsTracking)
            {
                return null;
            }

            return ZoneStates[zoneId].MapScore;
        }

        public void UpdateZoneLockState(int zoneId, ZoneLockState lockState = null)
        {
            if (!ZoneStates.ContainsKey(zoneId))
            {
                return;
            }

            ZoneStates[zoneId].UpdateLockState(lockState);
        }

        public void UpdateZoneAlertState(int zoneId, ZoneAlertState alertState = null)
        {
            if (!ZoneStates.ContainsKey(zoneId))
            {
                return;
            }

            ZoneStates[zoneId].UpdateAlertState(alertState);
        }

        public async Task<FacilityControlChange> UpdateZoneFacilityFaction(int zoneId, int facilityId, int factionId)
        {
            if (!ZoneStates.ContainsKey(zoneId) || !ZoneStates[zoneId].IsTracking)
            {
                return null;
            }

            await ZoneStates[zoneId].FacilityFactionChange(facilityId, factionId);

            return new FacilityControlChange
            {
                Region = ZoneStates[zoneId].GetRegionByFacilityId(facilityId),
                Score = ZoneStates[zoneId].MapScore
            };
        }
    }
}

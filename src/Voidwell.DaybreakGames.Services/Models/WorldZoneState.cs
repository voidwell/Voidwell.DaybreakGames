using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Models
{
    public class WorldZoneState
    {
        public int WorldId { get; private set; }
        public int ZoneId { get; private set; }
        public string Name { get; private set; }
        public bool IsTracking { get; private set; }

        public WorldZone Map { get; private set; }
        public MapScore MapScore { get; private set; }
        public Dictionary<int, int> MapRegionOwnership { get; private set; } = new Dictionary<int, int>();
        public ZoneLockState LockState { get; private set; }

        private ZoneAlertState _alertState { get; set; }
        private readonly SemaphoreSlim _facilityFactionChangeLock = new SemaphoreSlim(1);

        public WorldZoneState(int worldId, Zone zone)
        {
            WorldId = worldId;
            ZoneId = zone.Id;
            Name = zone.Code;

            IsTracking = false;
        }

        public WorldZoneState(int worldId, Zone zone, ZoneMap zoneMap, IEnumerable<ZoneRegionOwnership> ownership)
        {
            WorldId = worldId;
            ZoneId = zone.Id;
            Name = zone.Code;

            Setup(zoneMap, ownership);
        }

        public void Setup(ZoneMap zoneMap, IEnumerable<ZoneRegionOwnership> ownership)
        {
            Map = new WorldZone(zoneMap);

            foreach (var own in ownership)
            {
                MapRegionOwnership[own.RegionId] = own.FactionId;
            }

            PostMapUpdate();

            IsTracking = true;
        }

        public void DisableTracking()
        {
            IsTracking = false;
            UpdateLockState(null);
            UpdateAlertState();
        }

        public async Task FacilityFactionChange(int facilityId, int factionId)
        {
            await _facilityFactionChangeLock.WaitAsync();

            try
            {
                var region = Map.Regions.SingleOrDefault(r => r.FacilityId == facilityId);
                if (region != null)
                {
                    MapRegionOwnership[region.RegionId] = factionId;

                    PostMapUpdate();
                }
            }
            finally
            {
                _facilityFactionChangeLock.Release();
            }
        }

        public void UpdateLockState(ZoneLockState lockState)
        {
            LockState = lockState;

            if (LockState?.State == ZoneLockStateEnum.LOCKED)
            {
                UpdateAlertState();
            }
        }

        public void UpdateAlertState(ZoneAlertState alertState = null)
        {
            _alertState = alertState;
        }

        public IEnumerable<ZoneRegionOwnership> GetMapOwnership()
        {
            if (!IsTracking)
            {
                return null;
            }

            return Map.Regions
                .Select(a => new ZoneRegionOwnership(ZoneId, a.RegionId, MapRegionOwnership[a.RegionId]));
        }

        public ZoneAlertState GetAlertState()
        {
            if (_alertState != null) {
                if (LockState?.State == ZoneLockStateEnum.LOCKED ||
                    _alertState.MetagameEvent.Duration.HasValue && DateTime.UtcNow - _alertState.Timestamp > _alertState.MetagameEvent.Duration.Value)
                {
                    UpdateAlertState();
                }
            } 

            return _alertState;
        }

        public WorldZoneRegion GetRegionByFacilityId(int facilityId)
        {
            return Map.Regions.FirstOrDefault(r => r.FacilityId == facilityId);
        }

        private void PostMapUpdate()
        {
            MapScore = OwnershipCalculator.Calculate(Map, MapRegionOwnership);

            var warpgateFactions = Map.Warpgates.Select(a => MapRegionOwnership[a.RegionId]).Distinct();
            if (LockState == null && warpgateFactions.Count() == 1)
            {
                UpdateLockState(new ZoneLockState(DateTime.UtcNow, null, warpgateFactions.First()));
            }
            else if (LockState?.State == ZoneLockStateEnum.LOCKED && warpgateFactions.Count() > 1)
            {
                UpdateLockState(new ZoneLockState(DateTime.UtcNow));
            }
        }

        private static class OwnershipCalculator
        {
            public static MapScore Calculate(WorldZone zoneMap, Dictionary<int, int> ownership)
            {
                var _ownership = ownership;

                var territories = new[] { 0, 0, 0, 0, 0 };
                var ampStations = new[] { 0, 0, 0, 0, 0 };
                var techPlants = new[] { 0, 0, 0, 0, 0 };
                var bioLabs = new[] { 0, 0, 0, 0, 0 };
                var largeOutposts = new[] { 0, 0, 0, 0, 0 };
                var smallOutposts = new[] { 0, 0, 0, 0, 0 };

                foreach (var region in ownership)
                {
                    territories[region.Value]++;

                    var facilityType = zoneMap.Regions.FirstOrDefault(a => a.RegionId == region.Key)?.FacilityType;
                    switch (facilityType)
                    {
                        case "Amp Station":
                            ampStations[region.Value]++;
                            break;
                        case "Tech Plant":
                            techPlants[region.Value]++;
                            break;
                        case "Bio Lab":
                            bioLabs[region.Value]++;
                            break;
                        case "Large Outpost":
                            largeOutposts[region.Value]++;
                            break;
                        case "Small Outpost":
                            smallOutposts[region.Value]++;
                            break;
                    }
                }

                var connectedTerritories = CalculateConnectedTerritories(zoneMap, _ownership);

                return new MapScore
                {
                    Territories = new OwnershipScoreFactions(territories[1], territories[2], territories[3], territories[4], territories[0]),
                    ConnectedTerritories = new OwnershipScoreFactions(connectedTerritories[1], connectedTerritories[2], connectedTerritories[3], connectedTerritories[4], connectedTerritories[0]),
                    AmpStations = new OwnershipScoreFactions(ampStations[1], ampStations[2], ampStations[3], ampStations[4], ampStations[0]),
                    TechPlants = new OwnershipScoreFactions(techPlants[1], techPlants[2], techPlants[3], techPlants[4], techPlants[0]),
                    BioLabs = new OwnershipScoreFactions(bioLabs[1], bioLabs[2], bioLabs[3], bioLabs[4], bioLabs[0]),
                    LargeOutposts = new OwnershipScoreFactions(largeOutposts[1], largeOutposts[2], largeOutposts[3], largeOutposts[4], largeOutposts[0]),
                    SmallOutposts = new OwnershipScoreFactions(smallOutposts[1], smallOutposts[2], smallOutposts[3], smallOutposts[4], smallOutposts[0])
                };
            }

            private static int[] CalculateConnectedTerritories(WorldZone zoneMap, Dictionary<int, int> ownership)
            {
                var connectedTerritories = new[] { 0, 0, 0, 0, 0 };
                var factionsChecked = new Dictionary<int, bool>();

                foreach (var warpgate in zoneMap.Warpgates)
                {
                    var checkedRegions = new Dictionary<int, bool>();
                    var focusFaction = ownership[warpgate.RegionId];

                    if (!factionsChecked.ContainsKey(focusFaction))
                    {
                        factionsChecked.Add(focusFaction, true);

                        connectedTerritories[focusFaction]++;
                        checkedRegions.Add(warpgate.RegionId, true);

                        var factionRegions = ownership.Where(a => a.Value == focusFaction).ToDictionary(a => a.Key, a => a.Value);
                        connectedTerritories[focusFaction] += CountConnectedRegions(warpgate, checkedRegions, factionRegions);
                    }
                }

                return connectedTerritories;
            }

            private static int CountConnectedRegions(WorldZoneRegion source, Dictionary<int, bool> checkedRegions, Dictionary<int, int> factionRegions)
            {
                var connectedCount = 0;

                foreach (var linkedRegion in source.Links)
                {
                    if (checkedRegions.ContainsKey(linkedRegion.RegionId))
                    {
                        continue;
                    }

                    checkedRegions.Add(linkedRegion.RegionId, true);

                    if (factionRegions.ContainsKey(linkedRegion.RegionId))
                    {
                        connectedCount += 1 + CountConnectedRegions(linkedRegion, checkedRegions, factionRegions);
                    }
                }

                return connectedCount;
            }
        }
    }
}

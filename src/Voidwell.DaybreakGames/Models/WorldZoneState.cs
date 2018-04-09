using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Models
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

        private readonly SemaphoreSlim _facilityFactionChangeLock = new SemaphoreSlim(1);

        public WorldZoneState(int worldId, Zone zone)
        {
            WorldId = worldId;
            ZoneId = zone.Id;
            Name = zone.Code;

            IsTracking = false;
        }

        public WorldZoneState(int worldId, Zone zone, IEnumerable<FacilityLink> facilityLinks, IEnumerable<MapRegion> mapRegions, IEnumerable<MapOwnership> ownership)
        {
            WorldId = worldId;
            ZoneId = zone.Id;
            Name = zone.Code;

            Map = new WorldZone(facilityLinks, mapRegions);

            foreach (var own in ownership)
            {
                MapRegionOwnership[own.RegionId] = own.FactionId;
            }

            CalculateOwnership();

            IsTracking = true;
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

                    CalculateOwnership();
                }
            }
            finally
            {
                _facilityFactionChangeLock.Release();
            }
        }

        public void UpdateLockState(ZoneLockState lockState = null)
        {
            LockState = lockState;
        }

        public IEnumerable<ZoneRegionOwnership> GetMapOwnership()
        {
            if (!IsTracking)
            {
                return null;
            }

            return Map.Regions
                .Select(a => a as ZoneRegion)
                .Select(a => new ZoneRegionOwnership(a, MapRegionOwnership[a.RegionId]));
        }

        private void CalculateOwnership()
        {
            var score = new OwnershipCalculator(Map, MapRegionOwnership);

            MapScore = new MapScore
            {
                Territories = score.Territories,
                ConnectedTerritories = score.ConnectedTerritories,
                Percent = score.Percent,
                ConnectedPercent = score.ConnectedPercent
            };
        }

        private class OwnershipCalculator
        {
            public int[] Territories { get; private set; } = new[] { 0, 0, 0, 0 };
            public float[] Percent { get; private set; } = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
            public int[] ConnectedTerritories { get; private set; } = new[] { 0, 0, 0, 0 };
            public float[] ConnectedPercent { get; private set; } = new[] { 0f, 0f, 0f, 0f };

            private Dictionary<int, bool> _factionsChecked;
            private Dictionary<int, bool> _checkedRegions;
            private Dictionary<int, int> _ownership;
            private int _focusFaction;

            public OwnershipCalculator(WorldZone zoneMap, Dictionary<int, int> ownership)
            {
                _factionsChecked = new Dictionary<int, bool>();
                _ownership = ownership;

                foreach (var region in ownership)
                {
                    Territories[region.Value]++;
                }

                for(var i = 0; i < Territories.Count(); i++)
                {
                    Percent[i] = (float)Territories[i] / ownership.Count;
                }
                
                foreach (var warpgate in zoneMap.Warpgates)
                {
                    _checkedRegions = new Dictionary<int, bool>();
                    _focusFaction = ownership[warpgate.RegionId];

                    if (!_factionsChecked.ContainsKey(_focusFaction))
                    {
                        _factionsChecked.Add(_focusFaction, true);

                        ConnectedTerritories[_focusFaction]++;
                        _checkedRegions.Add(warpgate.RegionId, true);

                        CheckLinks(warpgate, warpgate.Links);
                    }
                }

                for (var i = 1; i < ConnectedTerritories.Count(); i++)
                {
                    ConnectedPercent[i] = (float)ConnectedTerritories[i] / ownership.Count;
                }

                ConnectedPercent[0] = 1 - (ConnectedPercent[1] + ConnectedPercent[2] + ConnectedPercent[3]);
            }

            private void CheckLinks(WorldZoneRegion root, List<WorldZoneRegion> links)
            {
                foreach (var link in links)
                {
                    if (_checkedRegions.ContainsKey(link.RegionId))
                    {
                        continue;
                    }

                    _checkedRegions.Add(link.RegionId, true);

                    if (_ownership[link.RegionId] == _focusFaction)
                    {
                        ConnectedTerritories[_focusFaction]++;

                        CheckLinks(root, link.Links);
                    }
                }
            }
        }
    }
}

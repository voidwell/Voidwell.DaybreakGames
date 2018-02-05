using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldZoneState
    {
        public int WorldId { get; private set; }
        public int ZoneId { get; private set; }
        public WorldZone Map { get; private set; }
        public MapScore MapScore { get; private set; }
        public Dictionary<int, int> MapRegionOwnership { get; private set; }

        public WorldZoneState(int worldId, int zoneId, IEnumerable<FacilityLink> facilityLinks, IEnumerable<MapRegion> mapRegions, IEnumerable<MapOwnership> ownership)
        {
            WorldId = worldId;
            ZoneId = zoneId;

            Map = new WorldZone(facilityLinks, mapRegions);

            foreach(var own in ownership)
            {
                MapRegionOwnership[own.RegionId] = own.FactionId;
            }

            CalculateOwnership();
        }

        public void FacilityFactionChange(int facilityId, int factionId)
        {
            var region = Map.Regions.SingleOrDefault(r => r.FacilityId == facilityId);
            if (region != null)
            {
                MapRegionOwnership[region.RegionId] = factionId;

                CalculateOwnership();
            }
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
            public float[] Percent { get; private set; } = new[] { 0f, 0f, 0f, 0f };
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

                foreach (var factionId in Territories)
                {
                    Percent[factionId] = Territories[factionId] / zoneMap.Regions.Count;
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

                        CheckLinks(root, links);
                    }
                }
            }
        }
    }
}

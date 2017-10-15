using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldZoneState
    {
        public string WorldId { get; private set; }
        public string ZoneId { get; private set; }
        public WorldZone Map { get; private set; }
        public MapScore MapScore { get; private set; }
        public Dictionary<string, int> MapRegionOwnership { get; private set; }

        public WorldZoneState(string worldId, string zoneId, IEnumerable<DbFacilityLink> facilityLinks, IEnumerable<DbMapRegion> mapRegions, IEnumerable<MapOwnership> ownership)
        {
            WorldId = worldId;
            ZoneId = zoneId;

            Map = new WorldZone(facilityLinks, mapRegions);

            foreach(var own in ownership)
            {
                MapRegionOwnership[own.RegionId] = int.TryParse(own.FactionId, out int iFaction) ? iFaction : 0;
            }

            CalculateOwnership();
        }

        public void FacilityFactionChange(string facilityId, string factionId)
        {
            var region = Map.Regions.SingleOrDefault(r => r.FacilityId == facilityId);
            if (region != null)
            {
                MapRegionOwnership[region.RegionId] = int.TryParse(factionId, out int iFaction) ? iFaction : 0;

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
            private Dictionary<string, bool> _checkedRegions;
            private Dictionary<string, int> _ownership;
            private int _focusFaction;

            public OwnershipCalculator(WorldZone zoneMap, Dictionary<string, int> ownership)
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
                    _checkedRegions = new Dictionary<string, bool>();
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

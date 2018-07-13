using System;
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

        public WorldZoneState(int worldId, Zone zone, ZoneMap zoneMap, IEnumerable<ZoneRegionOwnership> ownership)
        {
            WorldId = worldId;
            ZoneId = zone.Id;
            Name = zone.Code;

            Map = new WorldZone(zoneMap);

            foreach (var own in ownership)
            {
                MapRegionOwnership[own.RegionId] = own.FactionId;
            }

            CalculateOwnership();

            var warpgateFactions = Map.Warpgates.Select(a => MapRegionOwnership[a.RegionId]).Distinct();
            if (warpgateFactions.Count() == 1)
            {
                UpdateLockState(new ZoneLockState(DateTime.MinValue, null, warpgateFactions.First()));
            }

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
                .Select(a => new ZoneRegionOwnership(a.RegionId, MapRegionOwnership[a.RegionId]));
        }

        private void CalculateOwnership()
        {
            var calculator = new OwnershipCalculator(Map, MapRegionOwnership);
            MapScore = calculator.Score;
        }

        private class OwnershipCalculator
        {
            public MapScore Score = new MapScore();

            private int[] _territories { get; set; } = new[] { 0, 0, 0, 0 };
            private int[] _connectedTerritories { get; set; } = new[] { 0, 0, 0, 0 };
            private int[] _ampStations { get; set; } = new[] { 0, 0, 0, 0 };
            private int[] _techPlants { get; set; } = new[] { 0, 0, 0, 0 };
            private int[] _bioLabs { get; set; } = new[] { 0, 0, 0, 0 };

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
                    _territories[region.Value]++;

                    var facilityType = zoneMap.Regions.FirstOrDefault(a => a.RegionId == region.Key)?.FacilityType;
                    switch(facilityType)
                    {
                        case "Amp Station":
                            _ampStations[region.Value]++;
                            break;
                        case "Tech Plant":
                            _techPlants[region.Value]++;
                            break;
                        case "Bio Lab":
                            _bioLabs[region.Value]++;
                            break;
                    }
                }
                
                foreach (var warpgate in zoneMap.Warpgates)
                {
                    _checkedRegions = new Dictionary<int, bool>();
                    _focusFaction = ownership[warpgate.RegionId];

                    if (!_factionsChecked.ContainsKey(_focusFaction))
                    {
                        _factionsChecked.Add(_focusFaction, true);

                        _connectedTerritories[_focusFaction]++;
                        _checkedRegions.Add(warpgate.RegionId, true);

                        CheckLinks(warpgate, warpgate.Links);
                    }
                }

                Score.Territories = new OwnershipScoreFactions(_territories[1], _territories[2], _territories[3], _territories[0]);
                Score.ConnectedTerritories = new OwnershipScoreFactions(_connectedTerritories[1], _connectedTerritories[2], _connectedTerritories[3], _connectedTerritories[0]);
                Score.AmpStations = new OwnershipScoreFactions(_ampStations[1], _ampStations[2], _ampStations[3], _ampStations[0]);
                Score.TechPlants = new OwnershipScoreFactions(_techPlants[1], _techPlants[2], _techPlants[3], _techPlants[0]);
                Score.BioLabs = new OwnershipScoreFactions(_bioLabs[1], _bioLabs[2], _bioLabs[3], _bioLabs[0]);
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
                        _connectedTerritories[_focusFaction]++;

                        CheckLinks(root, link.Links);
                    }
                }
            }
        }
    }
}

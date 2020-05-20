using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.GameState
{
    public class ZoneMonitor : IZoneMonitor
    {
        private readonly IZoneService _zoneService;
        private readonly IMapService _mapService;

        private readonly Dictionary<int, WorldZone> _zoneLayouts = new Dictionary<int, WorldZone>();
        private readonly Dictionary<int, Dictionary<int, int>> _worldRegionOwnerships = new Dictionary<int, Dictionary<int, int>>();

        public ZoneMonitor(IZoneService zoneService, IMapService mapService)
        {
            _zoneService = zoneService;
            _mapService = mapService;
        }

        public async Task ZoneLayoutSetup()
        {
            var zones = await _zoneService.GetPlayableZones();

            var zoneMaps = zones.Select(a => (a.Id, task: _mapService.GetZoneMap(a.Id)));

            await Task.WhenAll(zoneMaps.Select(a => a.task));

            _zoneLayouts.Clear();
            zoneMaps.Where(a => a.task.Result != null).ToList().ForEach(a => _zoneLayouts.Add(a.Id, new WorldZone(a.task.Result)));
        }

        public async Task LoadAllWorldOwnerships(int worldId)
        {
            var zoneIds = _zoneLayouts.Keys.ToArray();
            var worldOwnerships = await _mapService.GetMapOwnership(worldId, zoneIds);

            var worldRegionOwnerships = worldOwnerships.ToDictionary(a => a.RegionId, a => a.FactionId);

            if (_worldRegionOwnerships.ContainsKey(worldId))
            {
                _worldRegionOwnerships[worldId].Clear();
            }

            _worldRegionOwnerships.Add(worldId, worldRegionOwnerships);
        }

        public async Task LoadWorldOwnership(int worldId, int zoneId)
        {
            var zoneOwnership = await _mapService.GetMapOwnership(worldId, zoneId);

            var worldRegionOwnerships = zoneOwnership.ToDictionary(a => a.RegionId, a => a.FactionId);

            if (!_worldRegionOwnerships.ContainsKey(worldId))
            {
                _worldRegionOwnerships.Add(worldId, new Dictionary<int, int>());
            }
            else if (_zoneLayouts.ContainsKey(zoneId))
            {
                GetRegionIds(zoneId).ToList().ForEach(a => _worldRegionOwnerships[worldId].Remove(a));
            }

            zoneOwnership.ToList().ForEach(a => _worldRegionOwnerships[worldId].Add(a.RegionId, a.FactionId));
        }

        public void ClearZoneStates(int worldId)
        {
            _worldRegionOwnerships.Remove(worldId);
        }

        public void ClearAllZoneStates()
        {
            _worldRegionOwnerships.Clear();
        }

        public void UpdateRegionOwnership(int worldId, int regionId, int newFactionId)
        {
            if (_worldRegionOwnerships.ContainsKey(worldId) && _worldRegionOwnerships[worldId].ContainsKey(regionId))
            {
                _worldRegionOwnerships[worldId][regionId] = newFactionId;
            }
        }

        public IEnumerable<(int regionId, int factionId)> GetZoneOwnership(int worldId, int zoneId)
        {
            return GetRegionIds(zoneId).Select(a => (a, _worldRegionOwnerships[worldId][a]));
        }

        public MapScore GetZoneScore(int worldId, int zoneId)
        {
            if (_worldRegionOwnerships.ContainsKey(worldId) && _zoneLayouts.ContainsKey(zoneId))
            {
                var zoneOwnerships = GetZoneOwnership(worldId, zoneId).ToDictionary(a => a.regionId, a => a.factionId);

                return OwnershipCalculator.Calculate(_zoneLayouts[zoneId], zoneOwnerships);
            }

            return null;
        }

        private IEnumerable<int> GetRegionIds(int zoneId)
        {
            return _zoneLayouts[zoneId]?.Regions.Select(a => a.RegionId);
        }
    }
}

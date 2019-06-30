using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.CensusStream.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldMonitor : IWorldMonitor
    {
        private readonly IZoneService _zoneService;
        private readonly IWorldService _worldService;
        private readonly IWorldEventsService _worldEventService;
        private readonly IMapService _mapService;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly ILogger<WorldMonitor> _logger;

        private static readonly ConcurrentDictionary<int, WorldState> _worldStates = new ConcurrentDictionary<int, WorldState>();
        private static readonly ConcurrentDictionary<int, Dictionary<int, Zone>> _retryingWorlds = new ConcurrentDictionary<int, Dictionary<int, Zone>>();

        public WorldMonitor(IWorldEventsService worldEventService, IZoneService zoneService, IWorldService worldService, IMapService mapService,
            IPlayerMonitor playerMonitor, ILogger<WorldMonitor> logger)
        {
            _zoneService = zoneService;
            _worldService = worldService;
            _worldEventService = worldEventService;
            _mapService = mapService;
            _playerMonitor = playerMonitor;
            _logger = logger;

            Task.Run(InitializeWorlds);
        }

        private async Task InitializeWorlds()
        {
            var worldsTask = _worldService.GetAllWorlds();
            var zonesTask = _zoneService.GetPlayableZones();

            await Task.WhenAll(worldsTask, zonesTask);

            foreach(var world in worldsTask.Result)
            {
                if (!_worldStates.ContainsKey(world.Id))
                {
                    var worldState = new WorldState(world.Id, world.Name);

                    zonesTask.Result.ToList()
                        .ForEach(zone => worldState.InitZoneState(zone));

                    _worldStates.TryAdd(world.Id, worldState);
                }
            }
        }

        public async Task SetWorldState(int worldId, string worldName, bool isOnline)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                _worldStates.TryAdd(worldId, new WorldState(worldId, worldName));
            }

            if (isOnline)
            {
                await SetWorldOnline(worldId);
            }
            else
            {
                await SetWorldOffline(worldId);
            }
        }

        private Task SetWorldOnline(int worldId)
        {
            _logger.LogInformation($"Set world {worldId} ONLINE");
            _worldStates[worldId].SetWorldOnline();
            return SetupWorldZones(worldId);
        }

        private Task SetWorldOffline(int worldId, bool preservePlayers = false)
        {
            _logger.LogInformation($"Set world {worldId} OFFLINE");
            _worldStates[worldId].SetWorldOffline();

            if (preservePlayers)
            {
                return Task.CompletedTask;
            }

            return _playerMonitor.ClearWorldAsync(worldId);
        }

        public async Task SetupWorldZones(int worldId)
        {
            if (!_worldStates[worldId].IsOnline)
            {
                if (_retryingWorlds.ContainsKey(worldId))
                {
                    _retryingWorlds.TryRemove(worldId, out _);
                }
                return;
            }

            var zones = _retryingWorlds.GetValueOrDefault(worldId)?.Values ?? await _zoneService.GetPlayableZones();

            await Task.WhenAll(zones.Select(zone => SetupWorldZone(worldId, zone)));

            if (_retryingWorlds.ContainsKey(worldId) && _retryingWorlds[worldId].Any())
            {
                SetupWorldDelay(worldId);
            }
            else if (_retryingWorlds.ContainsKey(worldId))
            {
                _retryingWorlds.TryRemove(worldId, out var value);
            }
        }

        public async Task<bool> SetupWorldZone(int worldId, int zoneId, bool retryAsync = false)
        {
            var zone = await _zoneService.GetZone(zoneId);
            if (zone == null)
            {
                throw new InvalidOperationException($"Invalid zone id {zoneId} for world {worldId} provided.");
            }

            return await SetupWorldZone(worldId, zone, retryAsync);
        }

        public Task ClearAllWorldStates()
        {
            foreach(var worldId in _worldStates.Keys)
            {
                SetWorldOffline(worldId, true);
            }

            return Task.CompletedTask;
        }

        public async Task<FacilityControlChange> UpdateFacilityControl(FacilityControl facilityControl)
        {
            if (!_worldStates.ContainsKey(facilityControl.WorldId) || facilityControl.ZoneId == null)
            {
                return null;
            }

            return await _worldStates[facilityControl.WorldId].UpdateZoneFacilityFaction(facilityControl.ZoneId.Value, facilityControl.FacilityId, facilityControl.NewFactionId);
        }

        public void UpdateZoneLock(int worldId, int zoneId, ZoneLockState lockState)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return;
            }

            _worldStates[worldId].UpdateZoneLockState(zoneId, lockState);
        }

        public void UpdateZoneAlert(int worldId, int zoneId, ZoneAlertState alertState = null)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return;
            }

            _worldStates[worldId].UpdateZoneAlertState(zoneId, alertState);
        }

        public MapScore GetTerritory(int worldId, int zoneId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return null;
            }

            return _worldStates[worldId].GetZoneMapScore(zoneId);
        }

        public async Task<IEnumerable<float>> GetTerritoryFromDate(int worldId, int zoneId, DateTime date)
        {
            var fcEvent = await _worldEventService.GetLatestFacilityControl(worldId, zoneId, date);

            var sum = fcEvent.ZoneControlVs.GetValueOrDefault() + fcEvent.ZoneControlNc.GetValueOrDefault() + fcEvent.ZoneControlTr.GetValueOrDefault() + fcEvent.ZoneControlNs.GetValueOrDefault();

            return new[] {
                100 - sum,
                fcEvent.ZoneControlVs.GetValueOrDefault(),
                fcEvent.ZoneControlNc.GetValueOrDefault(),
                fcEvent.ZoneControlTr.GetValueOrDefault(),
                fcEvent.ZoneControlNs.GetValueOrDefault()
            };
        }

        public async Task<IEnumerable<OnlineCharacter>> GetOnlineCharactersByWorld(int worldId)
        {
            if (!_worldStates.ContainsKey(worldId) || !_worldStates[worldId].IsOnline)
            {
                return Enumerable.Empty<OnlineCharacter>();
            }

            return await _playerMonitor.GetAllAsync(worldId);
        }

        public async Task<PopulationPeriod> GetZonePopulation(int worldId, int zoneId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return null;
            }

            if (!_worldStates[worldId].IsOnline)
            {
                return new PopulationPeriod();
            }

            var zonePlayers = await _playerMonitor.GetAllAsync(worldId, zoneId);

            var vsCount = zonePlayers.Count(a => a.Character.FactionId == 1);
            var ncCount = zonePlayers.Count(a => a.Character.FactionId == 2);
            var trCount = zonePlayers.Count(a => a.Character.FactionId == 3);
            var nsCount = zonePlayers.Count(a => a.Character.FactionId == 4);

            return new PopulationPeriod(vsCount, ncCount, trCount, nsCount);
        }

        public async Task<IEnumerable<WorldOnlineState>> GetWorldStates()
        {
            return (await Task.WhenAll(_worldStates.Select(a => GetWorldState(a.Key)))).OrderBy(a => a.Name);
        }

        public IEnumerable<ZoneRegionOwnership> GetZoneOwnership(int worldId, int zoneId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return null;
            }

            return _worldStates[worldId].GetZoneMapOwnership(zoneId);
        }

        public async Task<IEnumerable<ZoneRegionOwnership>> RefreshZoneOwnership(int worldId, int zoneId)
        {
            var setupSuccess = await SetupWorldZone(worldId, zoneId, true);
            if (!setupSuccess || !_worldStates.ContainsKey(worldId))
            {
                return null;
            }

            return _worldStates[worldId].GetZoneMapOwnership(zoneId);
        }

        public async Task<WorldOnlineState> GetWorldState(int worldId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return null;
            }

            var worldState = _worldStates[worldId];

            var worldPopulation = worldState.IsOnline ? await _playerMonitor.GetPlayerCountAsync(worldId) : 0;

            var zoneStates = new List<WorldOnlineZoneState>();
            foreach (var zoneState in worldState.GetZoneStates())
            {
                zoneState.Population = await GetZonePopulation(worldId, zoneState.Id);
                zoneStates.Add(zoneState);
            }

            return new WorldOnlineState
            {
                Id = worldId,
                Name = worldState.Name,
                IsOnline = worldState.IsOnline,
                OnlineCharacters = (int)worldPopulation,
                ZoneStates = zoneStates
            };
        }

        private async Task<bool> SetupWorldZone(int worldId, Zone zone, bool retry = true)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return false;
            }

            if (_worldStates[worldId].GetZoneState(zone.Id) == null)
            {
                _worldStates[worldId].InitZoneState(zone);
            }

            var ownershipTask = _mapService.GetMapOwnership(worldId, zone.Id);
            var zoneMapTask = _mapService.GetZoneMap(zone.Id);

            await Task.WhenAll(ownershipTask, zoneMapTask);

            var ownership = ownershipTask.Result;
            var zoneMap = zoneMapTask.Result;

            /*
            if (ownership == null)
            {
                _logger.LogInformation($"Ownship is null for worldId {worldId} zoneId {zone.Id}, attempting to use historical data");

                ownership = await _mapService.GetMapOwnershipFromHistory(worldId, zone.Id);
            }
            */

            if (ownership == null || zoneMap?.Regions == null || zoneMap.Links == null)
            {
                var errors = new List<string>();
                if (ownership == null) errors.Add("Ownership is null");
                if (zoneMap == null)
                {
                    errors.Add("ZoneMap is null");
                }
                else
                {
                    if (zoneMap.Regions == null) errors.Add("ZoneMap.Regions is null");
                    if (zoneMap.Links == null) errors.Add("ZoneMap.Links is null");
                }

                _logger.LogError(71612, $"{string.Join(", ", errors)} for worldId {worldId} zoneId {zone.Id}");

                if (retry)
                {
                    if (!_retryingWorlds.ContainsKey(worldId))
                    {
                        _retryingWorlds[worldId] = new Dictionary<int, Zone>();
                    }

                    _retryingWorlds[worldId][zone.Id] = zone;
                }

                return false;
            }

            if (!_worldStates[worldId].TrySetupZoneState(zone.Id, zoneMap, ownership))
            {
                return false;
            }

            if (_retryingWorlds.ContainsKey(worldId) && _retryingWorlds[worldId].ContainsKey(zone.Id))
            {
                _retryingWorlds[worldId].Remove(zone.Id);
            }

            var lockStater = await _mapService.GetZoneStateHistoricals();

            var lastLockState = lockStater.GetLastLockState(worldId, zone.Id);
            _worldStates[worldId].UpdateZoneLockState(zone.Id, lastLockState);

            return true;
        }

        private void SetupWorldDelay(int worldId)
        {
            Task.Run(() => Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(t => SetupWorldZonesRetry(worldId)));
        }

        private Task SetupWorldZonesRetry(int worldId)
        {
            if (!_retryingWorlds.ContainsKey(worldId))
            {
                return Task.CompletedTask;
            }

            if (!_retryingWorlds[worldId].Any())
            {
                _retryingWorlds.TryRemove(worldId, out _);
                return Task.CompletedTask;
            }

            return SetupWorldZones(worldId);
        }
    }
}

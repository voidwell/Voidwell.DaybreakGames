using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldMonitor : IWorldMonitor
    {
        private readonly IPlayerSessionRepository _playerSessionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IZoneService _zoneService;
        private readonly IWorldService _worldService;
        private readonly IMapService _mapService;
        private readonly ICharacterService _characterService;
        private readonly ICharacterUpdaterService _updaterService;
        private readonly ILogger<WorldMonitor> _logger;

        private static ConcurrentDictionary<int, WorldState> _worldStates = new ConcurrentDictionary<int, WorldState>();
        private static ConcurrentDictionary<int, Dictionary<int, Zone>> RetryingWorlds = new ConcurrentDictionary<int, Dictionary<int, Zone>>();

        public WorldMonitor(IPlayerSessionRepository playerSessionRepository, IEventRepository eventRepository,
            IZoneService zoneService, IWorldService worldService, IMapService mapService, ICharacterService characterService,
            ICharacterUpdaterService updaterService, ILogger<WorldMonitor> logger)
        {
            _playerSessionRepository = playerSessionRepository;
            _eventRepository = eventRepository;
            _zoneService = zoneService;
            _worldService = worldService;
            _mapService = mapService;
            _characterService = characterService;
            _updaterService = updaterService;
            _logger = logger;

            Task.Run(() => InitializeWorlds());
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
                SetWorldOffline(worldId);
            }
        }

        private async Task SetWorldOnline(int worldId)
        {
            _worldStates[worldId].SetWorldOnline();
            _logger.LogInformation($"Set world {worldId} ONLINE");
            await SetupWorldZones(worldId);
        }

        private void SetWorldOffline(int worldId)
        {
            _worldStates[worldId].SetWorldOffline();
            _logger.LogInformation($"Set world {worldId} OFFLINE");
        }

        public async Task SetupWorldZones(int worldId)
        {
            if (!_worldStates[worldId].IsOnline)
            {
                if (RetryingWorlds.ContainsKey(worldId))
                {
                    RetryingWorlds.TryRemove(worldId, out var value);
                }
                return;
            }

            var zones = RetryingWorlds.GetValueOrDefault(worldId)?.Values ?? await _zoneService.GetPlayableZones();

            await Task.WhenAll(zones.Select(zone => SetupWorldZone(worldId, zone)));

            if (RetryingWorlds.ContainsKey(worldId) && RetryingWorlds[worldId].Any())
            {
                SetupWorldDelay(worldId);
            }
            else if (RetryingWorlds.ContainsKey(worldId))
            {
                RetryingWorlds.TryRemove(worldId, out var value);
            }
        }

        public async Task<bool> SetupWorldZone(int worldId, int zoneId, bool retryAsync = false)
        {
            var zone = await _zoneService.GetZone(zoneId);
            if (zone == null)
            {
                throw new InvalidOperationException("Invalid zone id provided");
            }

            return await SetupWorldZone(worldId, zone, retryAsync);
        }

        public Task ClearAllWorldStates()
        {
            foreach(var worldId in _worldStates.Keys)
            {
                SetWorldOffline(worldId);
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

        public void UpdateZoneLock(int worldId, int zoneId, ZoneLockState lockState = null)
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
            var fcEvent = await _eventRepository.GetLatestFacilityControl(worldId, zoneId, date);            

            var sum = fcEvent.ZoneControlVs.GetValueOrDefault() + fcEvent.ZoneControlNc.GetValueOrDefault() + fcEvent.ZoneControlTr.GetValueOrDefault();

            return new[] {
                100 - sum,
                fcEvent.ZoneControlVs.GetValueOrDefault(),
                fcEvent.ZoneControlNc.GetValueOrDefault(),
                fcEvent.ZoneControlTr.GetValueOrDefault()
            };
        }

        public async Task SetPlayerOnlineState(string characterId, DateTime timestamp, bool isOnline)
        {
            var character = await _characterService.GetCharacter(characterId);
            if (character == null || !_worldStates.ContainsKey(character.WorldId))
            {
                return;
            }

            if (isOnline)
            {
                _worldStates[character.WorldId].SetPlayerOnline(character.Id, character.Name, character.FactionId, timestamp);
                return;
            }

            var onlineCharacter = _worldStates[character.WorldId].SetPlayerOffline(character.Id);
            if (onlineCharacter == null)
            {
                return;
            }

            var duration = timestamp - onlineCharacter.LoginDate;
            if (duration.TotalMinutes >= 5)
            {
                await _updaterService.AddToQueue(characterId);
            }

            var dataModel = new Data.Models.Planetside.PlayerSession
            {
                CharacterId = characterId,
                LoginDate = onlineCharacter.LoginDate,
                LogoutDate = timestamp,
                Duration = (int)duration.TotalMilliseconds
            };
            await _playerSessionRepository.AddAsync(dataModel);
        }

        public void SetPlayerLastSeen(int worldId, string characterId, DateTime timestamp, int zoneId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return;
            }

            _worldStates[worldId].SetPlayerLastSeen(characterId, zoneId, timestamp);
        }

        public IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(int worldId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return Enumerable.Empty<OnlineCharacter>();
            }

            return _worldStates[worldId].GetOnlinePlayers();
        }

        public ZonePopulation GetZonePopulation(int worldId, int zoneId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return null;
            }

            return _worldStates[worldId].GetZonePopulation(zoneId);
        }

        public IEnumerable<WorldOnlineState> GetWorldStates()
        {
            return _worldStates.Select(a => new WorldOnlineState {
                Id = a.Key,
                Name = a.Value.Name,
                IsOnline = a.Value.IsOnline,
                OnlineCharacters = a.Value.CountOnlinePlayers(),
                ZoneStates = a.Value.GetZoneStates()
            }).OrderBy(a => a.Name);
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

            if (ownership == null || zoneMap == null || zoneMap.Regions == null || zoneMap.Links == null)
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
                    if (!RetryingWorlds.ContainsKey(worldId))
                    {
                        RetryingWorlds[worldId] = new Dictionary<int, Zone>();
                    }

                    RetryingWorlds[worldId][zone.Id] = zone;
                }

                return false;
            }

            if (!_worldStates[worldId].TrySetupZoneState(zone.Id, zoneMap, ownership))
            {
                return false;
            }

            if (RetryingWorlds.ContainsKey(worldId) && RetryingWorlds[worldId].ContainsKey(zone.Id))
            {
                RetryingWorlds[worldId].Remove(zone.Id);
            }

            return true;
        }

        private void SetupWorldDelay(int worldId)
        {
            Task.Run(() => Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(t => SetupWorldZonesRetry(worldId)));
        }

        private Task SetupWorldZonesRetry(int worldId)
        {
            if (!RetryingWorlds.ContainsKey(worldId))
            {
                return Task.CompletedTask;
            }
            else if (!RetryingWorlds[worldId].Any())
            {
                RetryingWorlds.TryRemove(worldId, out var value);
                return Task.CompletedTask;
            }

            return SetupWorldZones(worldId);
        }
    }
}

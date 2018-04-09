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
        private readonly IMapService _mapService;
        private readonly ICharacterService _characterService;
        private readonly ICharacterUpdaterService _updaterService;
        private readonly ILogger<WorldMonitor> _logger;

        private static ConcurrentDictionary<int, WorldState> _worldStates = new ConcurrentDictionary<int, WorldState>();

        public WorldMonitor(IPlayerSessionRepository playerSessionRepository, IEventRepository eventRepository,
            IZoneService zoneService, IMapService mapService, ICharacterService characterService,
            ICharacterUpdaterService updaterService, ILogger<WorldMonitor> logger)
        {
            _playerSessionRepository = playerSessionRepository;
            _eventRepository = eventRepository;
            _zoneService = zoneService;
            _mapService = mapService;
            _characterService = characterService;
            _updaterService = updaterService;
            _logger = logger;
        }

        public async Task SetWorldState(int worldId, string worldName, bool isOnline)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                _worldStates.TryAdd(worldId, new WorldState
                {
                    Id = worldId,
                    Name = worldName,
                    IsOnline = false
                });
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
            _worldStates[worldId].ZoneStates.Clear();
            _worldStates[worldId].OnlinePlayers.Clear();
            _worldStates[worldId].IsOnline = true;

            _logger.LogInformation($"Set world {worldId} ONLINE");

            await SetupWorldZones(worldId);
        }

        private void SetWorldOffline(int worldId)
        {
            _worldStates[worldId].ZoneStates.Clear();
            _worldStates[worldId].OnlinePlayers.Clear();
            _worldStates[worldId].IsOnline = false;

            _logger.LogInformation($"Set world {worldId} OFFLINE");
        }

        public async Task SetupWorldZones(int worldId)
        {
            _worldStates[worldId].ZoneStates.Clear();

            var zones = await _zoneService.GetPlayableZones();

            var zoneStateWork = zones.Select(zone => CreateWorldZoneState(worldId, zone));

            await Task.WhenAll(zoneStateWork);

            if (zoneStateWork.Any(t => t.Result == null))
            {
                _logger.LogInformation(75625, "Failed to create world zone states");
                return;
            }

            foreach (var zoneState in zoneStateWork.Select(t => t.Result))
            {
                _worldStates[worldId].ZoneStates.TryAdd(zoneState.ZoneId, zoneState);
            }
        }

        public Task ClearAllWorldStates()
        {
            foreach(var worldId in _worldStates.Keys)
            {
                SetWorldOffline(worldId);
            }

            return Task.CompletedTask;
        }

        private readonly KeyedSemaphoreSlim _updateFacilityControlLock = new KeyedSemaphoreSlim();

        public async Task<FacilityControlChange> UpdateFacilityControl(FacilityControl facilityControl)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(facilityControl.WorldId, facilityControl.ZoneId.Value, out zoneState))
            {
                return null;
            }

            using (await _updateFacilityControlLock.WaitAsync($"{zoneState.WorldId}{zoneState.ZoneId}"))
            {
                await zoneState.FacilityFactionChange(facilityControl.FacilityId, facilityControl.NewFactionId);

                return new FacilityControlChange
                {
                    Region = zoneState.Map.Regions.FirstOrDefault(r => r.FacilityId == facilityControl.FacilityId),
                    Territory = zoneState.MapScore.ConnectedPercent
                };
            }
        }

        public void UpdateZoneLock(int worldId, int zoneId, ZoneLockState lockState = null)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(worldId, zoneId, out zoneState))
            {
                return;
            }

            zoneState.UpdateLockState(lockState);
        }

        public MapScore GetTerritory(int worldId, int zoneId)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(worldId, zoneId, out zoneState))
            {
                return null;
            }

            return zoneState.MapScore;
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
            if (character == null)
            {
                return;
            }

            var worldId = character.WorldId;
            if (!_worldStates.ContainsKey(worldId))
            {
                return;
            }

            if (isOnline)
            {
                _worldStates[worldId].OnlinePlayers.AddOrUpdate(characterId, new OnlineCharacter
                {
                    Character = new OnlineCharacterProfile
                    {
                        CharacterId = character.Id,
                        FactionId = character.FactionId,
                        Name = character.Name,
                        WorldId = worldId
                    },
                    LoginDate = timestamp
                },
                (key, onlineChar) =>
                {
                    onlineChar.LoginDate = timestamp;
                    return onlineChar;
                });
                return;
            }

            if (!_worldStates[worldId].OnlinePlayers.ContainsKey(characterId))
            {
                return;
            }

            var onlineCharacter = _worldStates[worldId].OnlinePlayers[characterId];

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

            _worldStates[worldId].OnlinePlayers.TryRemove(characterId, out OnlineCharacter offlineCharacter);
        }

        public IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(int worldId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return Enumerable.Empty<OnlineCharacter>();
            }

            return _worldStates[worldId].OnlinePlayers.Values;
        }

        public IEnumerable<WorldOnlineState> GetWorldStates()
        {
            return _worldStates.Select(a => new WorldOnlineState {
                Id = a.Key,
                Name = a.Value.Name,
                IsOnline = a.Value.IsOnline,
                OnlineCharacters = a.Value.OnlinePlayers.Count,
                ZoneStates = a.Value.ZoneStates.Select(b => new WorldOnlineZoneState
                {
                    Id = b.Key,
                    Name = b.Value.Name,
                    IsTracking = b.Value.IsTracking,
                    LockState = b.Value.LockState
                })
            });
        }

        public IEnumerable<ZoneRegionOwnership> GetZoneMapState(int worldId, int zoneId)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(worldId, zoneId, out zoneState))
            {
                return null;
            }

            return zoneState.GetMapOwnership();
        }

        private async Task<WorldZoneState> CreateWorldZoneState(int worldId, Zone zone)
        {
            var ownershipTask = _mapService.GetMapOwnership(worldId, zone.Id);
            var regionsTask = _mapService.GetMapRegions(zone.Id);
            var facilityLinksTask = _mapService.GetFacilityLinks(zone.Id);

            await Task.WhenAll(ownershipTask, regionsTask, facilityLinksTask);

            var ownership = ownershipTask.Result;
            var mapRegions = regionsTask.Result;
            var facilityLinks = facilityLinksTask.Result;

            if (ownership == null || mapRegions == null || facilityLinks == null)
            {
                var errors = new List<string>();
                if (ownership == null) errors.Add("Ownership is null");
                if (mapRegions == null) errors.Add("Map regions is null");
                if (facilityLinks == null) errors.Add("Facility Links is null");

                _logger.LogError(71612, $"{string.Join(", ", errors)} for worldId {worldId} zoneId {zone.Id}");

                return new WorldZoneState(worldId, zone);
            }

            return new WorldZoneState(worldId, zone, facilityLinks, mapRegions, ownership);
        }

        private static bool TryGetZoneState(int worldId, int zoneId, out WorldZoneState zoneState)
        {
            if (!_worldStates.ContainsKey(worldId) || !_worldStates[worldId].ZoneStates.ContainsKey(zoneId) || !_worldStates[worldId].ZoneStates[zoneId].IsTracking )
            {
                zoneState = null;
                return false;
            }

            zoneState = _worldStates[worldId].ZoneStates[zoneId];
            return true;
        }
    }
}

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
                    var worldState = new WorldState
                    {
                        Id = world.Id,
                        Name = world.Name,
                        IsOnline = false
                    };

                    zonesTask.Result.ToList()
                        .ForEach(zone => worldState.ZoneStates.TryAdd(zone.Id, new WorldZoneState(world.Id, zone)));

                    _worldStates.TryAdd(world.Id, worldState);
                }
            }
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
                    Score = zoneState.MapScore
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
            }).OrderBy(a => a.Name);
        }

        public IEnumerable<ZoneRegionOwnership> GetZoneOwnership(int worldId, int zoneId)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(worldId, zoneId, out zoneState))
            {
                return null;
            }

            return zoneState.GetMapOwnership() ?? Enumerable.Empty<ZoneRegionOwnership>();
        }

        private async Task<WorldZoneState> CreateWorldZoneState(int worldId, Zone zone)
        {
            var ownershipTask = _mapService.GetMapOwnership(worldId, zone.Id);
            var zoneMapTask = _mapService.GetZoneMap(zone.Id);

            await Task.WhenAll(ownershipTask, zoneMapTask);

            var ownership = ownershipTask.Result;
            var zoneMap = zoneMapTask.Result;

            if (ownership == null || zoneMap == null || zoneMap.Regions == null || zoneMap.Links == null)
            {
                var errors = new List<string>();
                if (ownership == null) errors.Add("Ownership is null");
                if (zoneMap == null) {
                    errors.Add("ZoneMap is null");
                }
                else
                {
                    if (zoneMap.Regions == null) errors.Add("ZoneMap.Regions is null");
                    if (zoneMap.Links == null) errors.Add("ZoneMap.Links is null");
                }

                _logger.LogError(71612, $"{string.Join(", ", errors)} for worldId {worldId} zoneId {zone.Id}");

                return new WorldZoneState(worldId, zone);
            }

            return new WorldZoneState(worldId, zone, zoneMap, ownership);
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

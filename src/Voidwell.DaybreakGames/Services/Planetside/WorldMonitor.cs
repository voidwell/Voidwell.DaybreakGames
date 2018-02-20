using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (!isOnline)
            {
                _worldStates[worldId].ZoneStates.Clear();
                _worldStates[worldId].OnlinePlayers.Clear();
                _worldStates[worldId].IsOnline = false;
            }
            else if (isOnline && !_worldStates[worldId].IsOnline)
            {
                var newState = await TryResetWorld(worldId);
                _worldStates[worldId].IsOnline = newState;
            }
        }

        public Task<bool> TryResetWorld(int worldId)
        {
            _worldStates[worldId].ZoneStates.Clear();
            _worldStates[worldId].OnlinePlayers.Clear();

            /*

            var zones = await _zoneService.GetAllZones();

            if (zones == null || zones.Any() == false)
            {
                return false;
            }

            var zoneStateWork = zones.Select(z => CreateWorldZoneState(worldId, z.Id));

            await Task.WhenAll(zoneStateWork);

            if (zoneStateWork.Any(t => t.Result == null))
            {
                return false;
            }

            foreach(var zoneState in zoneStateWork.Select(t => t.Result))
            {
                WorldStates[worldId].ZoneStates.Add(zoneState.ZoneId, zoneState);
            }
            */

            return Task.FromResult(true);
        }

        public FacilityControlChange UpdateFacilityControl(FacilityControl facilityControl)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(facilityControl.WorldId, facilityControl.ZoneId.Value, out zoneState))
            {
                return null;
            }

            zoneState.FacilityFactionChange(facilityControl.FacilityId, facilityControl.NewFactionId);

            return new FacilityControlChange
            {
                Region = zoneState.Map.Regions.FirstOrDefault(r => r.FacilityId == facilityControl.FacilityId),
                Territory = zoneState.MapScore.ConnectedPercent
            };
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
                _worldStates[worldId].OnlinePlayers.Add(characterId, new OnlineCharacter
                {
                    Character = new OnlineCharacterProfile
                    {
                        CharacterId = character.Id,
                        FactionId = character.FactionId,
                        Name = character.Name,
                        WorldId = worldId
                    },
                    LoginDate = timestamp
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

            _worldStates[worldId].OnlinePlayers.Remove(characterId);
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
                IsOnline = a.Value.IsOnline
            });
        }

        private async Task<WorldZoneState> CreateWorldZoneState(int worldId, int zoneId)
        {
            var ownershipTask = _mapService.GetMapOwnership(worldId, zoneId);
            var regionsTask = _mapService.GetMapRegions(zoneId);
            var facilityLinksTask = _mapService.GetFacilityLinks(zoneId);

            await Task.WhenAll(ownershipTask, regionsTask, facilityLinksTask);

            var ownership = ownershipTask.Result;
            var mapRegions = regionsTask.Result;
            var facilityLinks = facilityLinksTask.Result;

            if (ownership == null || mapRegions == null || facilityLinks == null)
            {
                return null;
            }

            return new WorldZoneState(worldId, zoneId, facilityLinks, mapRegions, ownership);
        }

        private static bool TryGetZoneState(int worldId, int zoneId, out WorldZoneState zoneState)
        {
            if (!_worldStates.ContainsKey(worldId) || !_worldStates[worldId].ZoneStates.ContainsKey(zoneId))
            {
                zoneState = null;
                return false;
            }

            zoneState = _worldStates[worldId].ZoneStates[zoneId];
            return true;
        }
    }
}

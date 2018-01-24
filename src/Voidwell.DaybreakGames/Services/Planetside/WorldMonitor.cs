using Microsoft.Extensions.Logging;
using System;
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
        private readonly IUpdaterService _updaterService;
        private readonly ILogger<WorldMonitor> _logger;

        private static Dictionary<string, WorldState> _worldStates = new Dictionary<string, WorldState>();

        public WorldMonitor(IPlayerSessionRepository playerSessionRepository, IEventRepository eventRepository,
            IZoneService zoneService, IMapService mapService, ICharacterService characterService,
            IUpdaterService updaterService, ILogger<WorldMonitor> logger)
        {
            _playerSessionRepository = playerSessionRepository;
            _eventRepository = eventRepository;
            _zoneService = zoneService;
            _mapService = mapService;
            _characterService = characterService;
            _updaterService = updaterService;
            _logger = logger;
        }

        public async Task SetWorldState(string worldId, string worldName, bool isOnline)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                _worldStates.Add(worldId, new WorldState
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

        public Task<bool> TryResetWorld(string worldId)
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
            if (!TryGetZoneState(facilityControl.WorldId, facilityControl.ZoneId, out zoneState))
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

        public MapScore GetTerritory(string worldId, string zoneId)
        {
            WorldZoneState zoneState;
            if (!TryGetZoneState(worldId, zoneId, out zoneState))
            {
                return null;
            }

            return zoneState.MapScore;
        }

        public async Task<IEnumerable<float>> GetTerritoryFromDate(string worldId, string zoneId, DateTime date)
        {
            var fcEvent = await _eventRepository.GetLatestFacilityControl(worldId, zoneId, date);            

            var sum = fcEvent.ZoneControlVs + fcEvent.ZoneControlNc + fcEvent.ZoneControlTr;

            return new[] { 100 - sum, fcEvent.ZoneControlVs, fcEvent.ZoneControlNc, fcEvent.ZoneControlTr };
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

            var dataModel = new DbPlayerSession
            {
                CharacterId = characterId,
                LoginDate = onlineCharacter.LoginDate,
                LogoutDate = timestamp,
                Duration = (int)duration.TotalMilliseconds
            };
            await _playerSessionRepository.AddAsync(dataModel);

            _worldStates[worldId].OnlinePlayers.Remove(characterId);
        }

        public IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(string worldId)
        {
            if (!_worldStates.ContainsKey(worldId))
            {
                return Enumerable.Empty<OnlineCharacter>();
            }

            return _worldStates[worldId].OnlinePlayers.Values;
        }

        public Dictionary<string, bool> GetWorldStates()
        {
            return _worldStates.ToDictionary(a => a.Key, a => a.Value.IsOnline);
        }

        private async Task<WorldZoneState> CreateWorldZoneState(string worldId, string zoneId)
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

        private static bool TryGetZoneState(string worldId, string zoneId, out WorldZoneState zoneState)
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

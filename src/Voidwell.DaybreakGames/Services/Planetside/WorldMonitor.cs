using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldMonitor : IWorldMonitor, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly IZoneService _zoneService;
        private readonly IMapService _mapService;
        private readonly ICharacterService _characterService;
        private readonly IUpdaterService _updaterService;

        public Dictionary<string, WorldState> WorldStates { get; private set; }

        public WorldMonitor(PS2DbContext ps2DbContext, IZoneService zoneService, IMapService mapService, ICharacterService characterService, IUpdaterService updaterService)
        {
            _ps2DbContext = ps2DbContext;
            _zoneService = zoneService;
            _mapService = mapService;
            _characterService = characterService;
            _updaterService = updaterService;
        }

        public async Task SetWorldState(string worldId, string worldName, bool isOnline)
        {
            if (!WorldStates.ContainsKey(worldId))
            {
                WorldStates.Add(worldId, new WorldState
                {
                    Id = worldId,
                    Name = worldName,
                    IsOnline = isOnline
                });
            }

            if (!isOnline)
            {
                WorldStates[worldId].ZoneStates.Clear();
                WorldStates[worldId].OnlinePlayers.Clear();
            }
            else if (isOnline && !WorldStates[worldId].IsOnline)
            {
                var newState = await TryResetWorld(worldId);
                WorldStates[worldId].IsOnline = newState;
            }
        }

        public async Task<bool> TryResetWorld(string worldId)
        {
            WorldStates[worldId].ZoneStates.Clear();
            WorldStates[worldId].OnlinePlayers.Clear();

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

            return true;
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
            var fcEvent = await _ps2DbContext.EventFacilityControls
                .OrderBy("Timestamp", SortDirection.Descending)
                .SingleOrDefaultAsync(c => c.WorldId == worldId && c.ZoneId == zoneId && c.Timestamp <= date);

            var sum = fcEvent.ZoneControlVs + fcEvent.ZoneControlNc + fcEvent.ZoneControlTr;

            return new[] { 100 - sum, fcEvent.ZoneControlVs, fcEvent.ZoneControlNc, fcEvent.ZoneControlTr };
        }

        private async Task<WorldZoneState> CreateWorldZoneState(string worldId, string zoneId)
        {
            var ownershipTask = _mapService.GetMapOwnership(worldId, zoneId);
            var regionsTask =_mapService.GetMapRegions(zoneId);
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

        private bool TryGetZoneState(string worldId, string zoneId, out WorldZoneState zoneState)
        {
            if (!WorldStates.ContainsKey(worldId) || !WorldStates[worldId].ZoneStates.ContainsKey(zoneId))
            {
                zoneState = null;
                return false;
            }

            zoneState = WorldStates[worldId].ZoneStates[zoneId];
            return true;
        }

        public IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(string worldId)
        {
            if (!WorldStates.ContainsKey(worldId))
            {
                return null;
            }

            return WorldStates[worldId].OnlinePlayers.Values;
        }

        public async Task SetPlayerOnlineState(string characterId, DateTime timestamp, bool isOnline)
        {
            var character = await _characterService.GetCharacter(characterId);
            var worldId = character.WorldId;

            if (character == null || !WorldStates.ContainsKey(worldId))
            {
                return;
            }

            if (isOnline)
            {
                WorldStates[worldId].OnlinePlayers.Add(characterId, new OnlineCharacter
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

            if (!WorldStates[worldId].OnlinePlayers.ContainsKey(characterId))
            {
                return;
            }

            var onlineCharacter = WorldStates[worldId].OnlinePlayers[characterId];

            var duration = timestamp - onlineCharacter.LoginDate;
            if (duration.Minutes >= 300000)
            {
                await _updaterService.AddToQueue(characterId);
            }

            var dataModel = new DbPlayerSession
            {
                CharacterId = characterId,
                LoginDate = onlineCharacter.LoginDate,
                LogoutDate = timestamp,
                Duration = duration.Milliseconds
            };

            _ps2DbContext.PlayerSessions.Add(dataModel);
            await _ps2DbContext.SaveChangesAsync();

            WorldStates[worldId].OnlinePlayers.Remove(characterId);
        }

        public Dictionary<string, bool> GetWorldStates()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}

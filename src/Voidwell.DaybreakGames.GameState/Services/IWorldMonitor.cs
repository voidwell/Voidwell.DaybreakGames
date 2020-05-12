using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;
using Voidwell.DaybreakGames.GameState.CensusStream.Models;
using Voidwell.DaybreakGames.GameState.Models;

namespace Voidwell.DaybreakGames.GameState.Services
{
    public interface IWorldMonitor
    {
        Task<IEnumerable<ZoneRegionOwnership>> RefreshZoneOwnership(int worldId, int zoneId);
        void UpdateZoneAlert(int worldId, int zoneId, ZoneAlertState alertState = null);
        Task SetWorldState(int worldId, string worldName, bool isOnline);
        Task SetupWorldZones(int worldId);
        Task<bool> SetupWorldZone(int worldId, int zoneId, bool retryAsync = false);
        Task ClearAllWorldStates();
        Task<FacilityControlChange> UpdateFacilityControl(FacilityControl facilityControl);
        void UpdateZoneLock(int worldId, int zoneId, ZoneLockState lockState);
        MapScore GetTerritory(int worldId, int zoneId);
        Task<IEnumerable<float>> GetTerritoryFromDate(int worldId, int zoneId, DateTime date);
        Task<IEnumerable<OnlineCharacter>> GetOnlineCharactersByWorld(int worldId);
        Task<PopulationPeriod> GetZonePopulation(int worldId, int zoneId);
        Task<IEnumerable<WorldOnlineState>> GetWorldStates();
        IEnumerable<ZoneRegionOwnership> GetZoneOwnership(int worldId, int zoneId);
        Task<WorldOnlineState> GetWorldState(int worldId);
    }
}
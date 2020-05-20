using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Models;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWorldMonitor
    {
        Task SetWorldState(int worldId, string worldName, bool isOnline);
        Task<FacilityControlChange> UpdateFacilityControl(FacilityControl facilityControl);
        void UpdateZoneLock(int worldId, int zoneId, ZoneLockState lockState = null);
        void UpdateZoneAlert(int worldId, int zoneId, ZoneAlertState alertState = null);
        MapScore GetTerritory(int worldId, int zoneId);
        Task<IEnumerable<float>> GetTerritoryFromDate(int worldId, int zoneId, DateTime date);
        Task<IEnumerable<OnlineCharacter>> GetOnlineCharactersByWorld(int worldId);
        Task<IEnumerable<WorldOnlineState>> GetWorldStates();
        Task<WorldOnlineState> GetWorldState(int worldId);
        IEnumerable<ZoneRegionOwnership> GetZoneOwnership(int worldId, int zoneId);
        Task<IEnumerable<ZoneRegionOwnership>> RefreshZoneOwnership(int worldId, int zoneId);
        Task ClearAllWorldStates();
        Task SetupWorldZones(int worldId);
        Task<bool> SetupWorldZone(int worldId, int zoneId, bool retryAsync = false);
        Task<PopulationPeriod> GetZonePopulation(int worldId, int zoneId);
        IEnumerable<int> GetOnlineWorldIds();
    }
}

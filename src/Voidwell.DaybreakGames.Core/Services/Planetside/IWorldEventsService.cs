using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IWorldEventsService
    {
        Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date);
        Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
        Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
        Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
        Task<IEnumerable<DailyWeaponStats>> GetDailyWeaponAggregatesByWeaponIdAsync(int weaponId, DateTime start, DateTime end);
        Task<IEnumerable<ContinentLock>> GetAllLatestZoneLocks();
        Task<IEnumerable<ContinentUnlock>> GetAllLatestZoneUnlocks();
        Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime? end);
        Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end);
        Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end);
        Task<IEnumerable<BattlerankUp>> GetBattleRankUpEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end);
        Task<IEnumerable<PlayerLogin>> GetPlayerLoginEventsAsync(int worldId, DateTime startDate, DateTime? endDate);
        Task<IEnumerable<PlayerLogout>> GetPlayerLogoutEventsAsync(int worldId, DateTime startDate, DateTime? endDate);
        Task<PlayerLogin> GetLastPlayerLoginEventAsync(string characterId);
        Task<PlayerLogout> GetLastPlayerLogoutEventAsync(string characterId);
        Task<IEnumerable<VehicleDestroy>> GetVehicleDestroyEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end);
        Task<IEnumerable<GainExperience>> GetHealExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId= null);
        Task<IEnumerable<GainExperience>> GetReviveExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
        Task<IEnumerable<GainExperience>> GetRoadkillExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
        Task<IEnumerable<GainExperience>> GetSquadBeaconKillExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
    }
}
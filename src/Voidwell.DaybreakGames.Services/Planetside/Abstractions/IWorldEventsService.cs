using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface IWorldEventsService
{
    Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date);
    Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
    Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
    Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
    Task<IEnumerable<DailyWeaponStats>> GetDailyWeaponAggregatesByWeaponIdAsync(int weaponId, DateTime start, DateTime end);
    Task<IEnumerable<ContinentLock>> GetAllLatestZoneLocks();
    Task<IEnumerable<ContinentUnlock>> GetAllLatestZoneUnlocks();
    Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime? end);
    Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end);
    Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end);
    Task<IEnumerable<BattlerankUp>> GetBattleRankUpEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end);
    Task<IEnumerable<PlayerLogin>> GetPlayerLoginEventsAsync(int worldId, DateTime startDate, DateTime? endDate);
    Task<IEnumerable<PlayerLogout>> GetPlayerLogoutEventsAsync(int worldId, DateTime startDate, DateTime? endDate);
    Task<PlayerLogin> GetLastPlayerLoginEventAsync(long characterId);
    Task<PlayerLogout> GetLastPlayerLogoutEventAsync(long characterId);
    Task<IEnumerable<VehicleDestroy>> GetVehicleDestroyEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end);
}

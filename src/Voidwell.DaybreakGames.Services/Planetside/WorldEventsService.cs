using AsyncKeyedLock;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class WorldEventsService : IWorldEventsService, IDisposable
{
    private readonly IEventRepository _eventRepository;

    private readonly AsyncKeyedLocker<string> _facilityControlsByDateLock = new();

    public WorldEventsService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public Task<IEnumerable<ContinentLock>> GetAllLatestZoneLocks()
    {
        return _eventRepository.GetAllLatestZoneLocks();
    }

    public Task<IEnumerable<ContinentUnlock>> GetAllLatestZoneUnlocks()
    {
        return _eventRepository.GetAllLatestZoneUnlocks();
    }

    public Task<IEnumerable<BattlerankUp>> GetBattleRankUpEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end)
    {
        return _eventRepository.GetBattleRankUpEventsForCharacterIdByDateAsync(characterId, start, end);
    }

    public Task<IEnumerable<DailyWeaponStats>> GetDailyWeaponAggregatesByWeaponIdAsync(int weaponId, DateTime start, DateTime end)
    {
        return _eventRepository.GetDailyWeaponAggregatesByWeaponIdAsync(weaponId, start, end);
    }

    public Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
    {
        return _eventRepository.GetDeathEventsByDateAsync(worldId, startDate, endDate, zoneId);
    }

    public Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime? end)
    {
        return _eventRepository.GetDeathEventsForCharacterIdByDateAsync(characterId, start, end);
    }

    public Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end)
    {
        return _eventRepository.GetFacilityCaptureEventsForCharacterIdByDateAsync(characterId, start, end);
    }

    public async Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
    {
        using (await _facilityControlsByDateLock.LockAsync($"{worldId}_{startDate}_{endDate}_{zoneId}"))
        {
            return await _eventRepository.GetFacilityControlsByDateAsync(worldId, startDate, endDate, zoneId);
        }
    }

    public Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end)
    {
        return _eventRepository.GetFacilityDefendEventsForCharacterIdByDateAsync(characterId, start, end);
    }

    public Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date)
    {
        return _eventRepository.GetLatestFacilityControl(worldId, zoneId, date)!;
    }

    public Task<IEnumerable<PlayerLogin>> GetPlayerLoginEventsAsync(int worldId, DateTime startDate, DateTime? endDate)
    {
        return _eventRepository.GetPlayerLoginEventsAsync(worldId, startDate, endDate);
    }

    public Task<IEnumerable<PlayerLogout>> GetPlayerLogoutEventsAsync(int worldId, DateTime startDate, DateTime? endDate)
    {
        return _eventRepository.GetPlayerLogoutEventsAsync(worldId, startDate, endDate);
    }

    public Task<PlayerLogin> GetLastPlayerLoginEventAsync(long characterId)
    {
        return _eventRepository.GetLastPlayerLoginEventAsync(characterId)!;
    }

    public Task<PlayerLogout> GetLastPlayerLogoutEventAsync(long characterId)
    {
        return _eventRepository.GetLastPlayerLogoutEventAsync(characterId)!;
    }

    public Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
    {
        return _eventRepository.GetVehicleDeathEventsByDateAsync(worldId, startDate, endDate, zoneId);
    }

    public Task<IEnumerable<VehicleDestroy>> GetVehicleDestroyEventsForCharacterIdByDateAsync(long characterId, DateTime start, DateTime end)
    {
        return _eventRepository.GetVehicleDestroyEventsForCharacterIdByDateAsync(characterId, start, end);
    }

    public void Dispose()
    {
        _facilityControlsByDateLock.Dispose();
    }
}

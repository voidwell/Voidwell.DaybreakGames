using AsyncKeyedLock;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldEventsService : IWorldEventsService
    {
        private readonly IEventRepository _eventRepository;

        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        public WorldEventsService(AsyncKeyedLocker<string> asyncKeyedLocker, IEventRepository eventRepository)
        {
            _asyncKeyedLocker = asyncKeyedLocker;
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

        public Task<IEnumerable<BattlerankUp>> GetBattleRankUpEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
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

        public Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime? end)
        {
            return _eventRepository.GetDeathEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
        {
            return _eventRepository.GetFacilityCaptureEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public async Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
        {
            using (await _asyncKeyedLocker.LockAsync($"{worldId}_{startDate}_{endDate}_{zoneId}").ConfigureAwait(false))
            {
                return await _eventRepository.GetFacilityControlsByDateAsync(worldId, startDate, endDate, zoneId);
            }
        }

        public Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
        {
            return _eventRepository.GetFacilityDefendEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date)
        {
            return _eventRepository.GetLatestFacilityControl(worldId, zoneId, date);
        }

        public Task<IEnumerable<PlayerLogin>> GetPlayerLoginEventsAsync(int worldId, DateTime startDate, DateTime? endDate)
        {
            return _eventRepository.GetPlayerLoginEventsAsync(worldId, startDate, endDate);
        }

        public Task<IEnumerable<PlayerLogout>> GetPlayerLogoutEventsAsync(int worldId, DateTime startDate, DateTime? endDate)
        {
            return _eventRepository.GetPlayerLogoutEventsAsync(worldId, startDate, endDate);
        }

        public Task<PlayerLogin> GetLastPlayerLoginEventAsync(string characterId)
        {
            return _eventRepository.GetLastPlayerLoginEventAsync(characterId);
        }

        public Task<PlayerLogout> GetLastPlayerLogoutEventAsync(string characterId)
        {
            return _eventRepository.GetLastPlayerLogoutEventAsync(characterId);
        }

        public Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            return _eventRepository.GetVehicleDeathEventsByDateAsync(worldId, startDate, endDate, zoneId);
        }

        public Task<IEnumerable<VehicleDestroy>> GetVehicleDestroyEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
        {
            return _eventRepository.GetVehicleDestroyEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public Task<IEnumerable<GainExperience>> GetHealExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            return _eventRepository.GetExperienceByDateAsync(Experience.Heal, worldId, startDate, endDate, zoneId);
        }

        public Task<IEnumerable<GainExperience>> GetReviveExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
        {
            return _eventRepository.GetExperienceByDateAsync(Experience.Revive, worldId, startDate, endDate, zoneId);
        }

        public Task<IEnumerable<GainExperience>> GetRoadkillExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
        {
            return _eventRepository.GetExperienceByDateAsync(Experience.Roadkill, worldId, startDate, endDate, zoneId);
        }

        public Task<IEnumerable<GainExperience>> GetSquadBeaconKillExperienceEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
        {
            return _eventRepository.GetExperienceByDateAsync(Experience.SquadBeaconKill, worldId, startDate, endDate, zoneId);
        }

        private static class Experience
        {
            public const int Heal = 4;
            public const int Revive = 7;
            public const int Roadkill = 26;
            public const int SquadBeaconKill = 270;
        }
    }
}

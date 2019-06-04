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

        public Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
        {
            return _eventRepository.GetDeathEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
        {
            return _eventRepository.GetFacilityCaptureEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate)
        {
            return _eventRepository.GetFacilityControlsByDateAsync(worldId, startDate, endDate, zoneId);
        }

        public Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(string characterId, DateTime start, DateTime end)
        {
            return _eventRepository.GetFacilityDefendEventsForCharacterIdByDateAsync(characterId, start, end);
        }

        public Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date)
        {
            return _eventRepository.GetLatestFacilityControl(worldId, zoneId, date);
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
            public static readonly int Heal = 4;
            public static readonly int Revive = 7;
            public static readonly int Roadkill = 26;
            public static readonly int SquadBeaconKill = 270;
        }
    }
}

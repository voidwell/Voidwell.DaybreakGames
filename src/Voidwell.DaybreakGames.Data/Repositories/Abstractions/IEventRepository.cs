﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IEventRepository
    {
        Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime startDate, DateTime? endDate);
        Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task<IEnumerable<BattlerankUp>> GetBattleRankUpEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task<IEnumerable<VehicleDestroy>> GetVehicleDestroyEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task AddAsync<T>(T entity) where T : class;
        Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date);
        Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
        Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
        Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
        Task<IEnumerable<DailyWeaponStats>> GetDailyWeaponAggregatesByWeaponIdAsync(int itemId, DateTime start, DateTime end);
        Task<IEnumerable<ContinentUnlock>> GetAllLatestZoneUnlocks();
        Task<IEnumerable<ContinentLock>> GetAllLatestZoneLocks();
        Task<IEnumerable<GainExperience>> GetExperienceByDateAsync(int experienceId, int worldId, DateTime startDate, DateTime? endDate, int? zoneId);
        Task<IEnumerable<PlayerLogin>> GetPlayerLoginEventsAsync(int worldId, DateTime startDate, DateTime? endDate);
        Task<IEnumerable<PlayerLogout>> GetPlayerLogoutEventsAsync(int worldId, DateTime startDate, DateTime? endDate);
        Task<PlayerLogin> GetLastPlayerLoginEventAsync(string characterId);
        Task<PlayerLogout> GetLastPlayerLogoutEventAsync(string characterId);
    }
}
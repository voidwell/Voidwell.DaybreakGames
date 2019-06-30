using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public EventRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                dbContext.Add<T>(entity);

                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when ((ex.InnerException as PostgresException)?.SqlState == "23505")
                {
                    // Ignore unique constraint errors (https://www.postgresql.org/docs/current/static/errcodes-appendix.html)
                    return;
                }
            }
        }

        public async Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                var query = dbContext.EventDeaths.Where(e => e.WorldId == worldId && e.Timestamp < endDate && e.Timestamp > startDate);
                if (zoneId.HasValue)
                {
                    query = query.Where(e => e.ZoneId == zoneId);
                }

                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime startDate, DateTime? endDate)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                var query = from e in dbContext.EventDeaths

                             join weapon in dbContext.Items on e.AttackerWeaponId equals weapon.Id into weaponQ
                             from weapon in weaponQ.DefaultIfEmpty()

                             join attackerCharacter in dbContext.Characters on e.AttackerCharacterId equals attackerCharacter.Id into attackerCharacterQ
                             from attackerCharacter in attackerCharacterQ.DefaultIfEmpty()

                             join victimCharacter in dbContext.Characters on e.CharacterId equals victimCharacter.Id into victimCharacterQ
                             from victimCharacter in victimCharacterQ.DefaultIfEmpty()

                             where (e.AttackerCharacterId == characterId || e.CharacterId == characterId) && e.Timestamp > startDate && e.Timestamp < endDate
                            select new Death
                             {
                                 Timestamp = e.Timestamp,
                                 WorldId = e.WorldId,
                                 ZoneId = e.ZoneId,
                                 IsHeadshot = e.IsHeadshot,
                                 AttackerCharacterId = e.AttackerCharacterId,
                                 AttackerOutfitId = e.AttackerOutfitId,
                                 AttackerFireModeId = e.AttackerFireModeId,
                                 AttackerLoadoutId = e.AttackerLoadoutId,
                                 AttackerVehicleId = e.AttackerVehicleId,
                                 AttackerWeaponId = e.AttackerWeaponId,
                                 CharacterOutfitId = e.CharacterOutfitId,
                                 CharacterId = e.CharacterId,
                                 CharacterLoadoutId = e.CharacterLoadoutId,

                                 AttackerCharacter = attackerCharacter,
                                 Character = victimCharacter,
                                 AttackerWeapon = weapon
                             };

                var result = query.ToList();
                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<PlayerFacilityCapture>> GetFacilityCaptureEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from e in dbContext.PlayerFacilityCaptureEvents

                            join facility in dbContext.MapRegions on e.FacilityId equals facility.FacilityId into facilityQ
                            from facility in facilityQ.DefaultIfEmpty()

                            where e.CharacterId == characterId && e.Timestamp > lower && e.Timestamp < upper
                            select new PlayerFacilityCapture
                            {
                                Timestamp = e.Timestamp,
                                WorldId = e.WorldId,
                                ZoneId = e.ZoneId,
                                CharacterId = e.CharacterId,
                                FacilityId = e.FacilityId,
                                OutfitId = e.OutfitId,

                                Facility = facility
                            };

                var result = query.ToList();
                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<PlayerFacilityDefend>> GetFacilityDefendEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from e in dbContext.PlayerFacilityDefendEvents

                            join facility in dbContext.MapRegions on e.FacilityId equals facility.FacilityId into facilityQ
                            from facility in facilityQ.DefaultIfEmpty()

                            where e.CharacterId == characterId && e.Timestamp > lower && e.Timestamp < upper
                            select new PlayerFacilityDefend
                            {
                                Timestamp = e.Timestamp,
                                WorldId = e.WorldId,
                                ZoneId = e.ZoneId,
                                CharacterId = e.CharacterId,
                                FacilityId = e.FacilityId,
                                OutfitId = e.OutfitId,

                                Facility = facility
                            };

                var result = query.ToList();
                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<BattlerankUp>> GetBattleRankUpEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from e in dbContext.BattleRankUpEvents

                            where e.CharacterId == characterId && e.Timestamp > lower && e.Timestamp < upper
                            select new BattlerankUp
                            {
                                Timestamp = e.Timestamp,
                                WorldId = e.WorldId,
                                ZoneId = e.ZoneId,
                                CharacterId = e.CharacterId,
                                BattleRank = e.BattleRank
                            };

                var result = query.ToList();
                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<VehicleDestroy>> GetVehicleDestroyEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from e in dbContext.EventVehicleDestroys

                            join weapon in dbContext.Items on e.AttackerWeaponId equals weapon.Id into weaponQ
                            from weapon in weaponQ.DefaultIfEmpty()

                            join attackerCharacter in dbContext.Characters on e.AttackerCharacterId equals attackerCharacter.Id into attackerCharacterQ
                            from attackerCharacter in attackerCharacterQ.DefaultIfEmpty()

                            join victimCharacter in dbContext.Characters on e.CharacterId equals victimCharacter.Id into victimCharacterQ
                            from victimCharacter in victimCharacterQ.DefaultIfEmpty()

                            join attackerVehicle in dbContext.Vehicles on e.AttackerVehicleId equals attackerVehicle.Id into attackerVehicleQ
                            from attackerVehicle in attackerVehicleQ.DefaultIfEmpty()

                            join victimVehicle in dbContext.Vehicles on e.VehicleId equals victimVehicle.Id into victimVehicleQ
                            from victimVehicle in victimVehicleQ.DefaultIfEmpty()

                            join facility in dbContext.MapRegions on e.FacilityId equals facility.FacilityId into facilityQ
                            from facility in facilityQ.DefaultIfEmpty()

                            where (e.AttackerCharacterId == characterId || e.CharacterId == characterId) && e.Timestamp > lower && e.Timestamp < upper
                            select new VehicleDestroy
                            {
                                Timestamp = e.Timestamp,
                                WorldId = e.WorldId,
                                ZoneId = e.ZoneId,
                                AttackerCharacterId = e.AttackerCharacterId,
                                AttackerLoadoutId = e.AttackerLoadoutId,
                                AttackerVehicleId = e.AttackerVehicleId,
                                AttackerWeaponId = e.AttackerWeaponId,
                                CharacterId = e.CharacterId,
                                FacilityId = e.FacilityId,
                                FactionId = e.FactionId,
                                VehicleId = e.VehicleId,
                                
                                AttackerCharacter = attackerCharacter,
                                Character = victimCharacter,
                                AttackerWeapon = weapon,
                                AttackerVehicle = attackerVehicle,
                                VictimVehicle = victimVehicle,
                                Facility = facility
                            };

                var result = query.ToList();
                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                var query = dbContext.EventFacilityControls.Where(e => e.WorldId == worldId && e.Timestamp < endDate && e.Timestamp > startDate);
                if (zoneId.HasValue)
                {
                    query = query.Where(e => e.ZoneId == zoneId);
                }

                return await query.ToListAsync();
            }
        }

        public async Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from control in dbContext.EventFacilityControls
                            where control.WorldId == worldId && control.ZoneId == zoneId && control.Timestamp <= date
                            orderby control.Timestamp descending
                            select control;

                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                var query = dbContext.EventVehicleDestroys.Where(e => e.WorldId == worldId && e.Timestamp < endDate && e.Timestamp > startDate);
                if (zoneId.HasValue)
                {
                    query = query.Where(e => e.ZoneId == zoneId);
                }

                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<DailyWeaponStats>> GetDailyWeaponAggregatesByWeaponIdAsync(int itemId, DateTime start, DateTime end)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.DailyWeaponStats.Where(a => a.WeaponId == itemId && a.Date >= start && a.Date <= end)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<ContinentUnlock>> GetAllLatestZoneUnlocks()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.ContinentUnlockEvents
                    .GroupBy(a => new { a.ZoneId, a.WorldId })
                    .Select(a => a.OrderByDescending(b => b.Timestamp).First())
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<ContinentLock>> GetAllLatestZoneLocks()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.ContinentLockEvents
                    .GroupBy(a => new { a.ZoneId, a.WorldId })
                    .Select(a => a.OrderByDescending(b => b.Timestamp).First())
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<GainExperience>> GetExperienceByDateAsync(int experienceId, int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                var query = dbContext.GainExperienceEvents.Where(e => e.ExperienceId == experienceId && e.WorldId == worldId && e.Timestamp < endDate && e.Timestamp > startDate);
                if (zoneId.HasValue)
                {
                    query = query.Where(e => e.ZoneId == zoneId);
                }

                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<PlayerLogin>> GetPlayerLoginEventsAsync(int worldId, DateTime startDate, DateTime? endDate)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                return await dbContext.PlayerLoginEvents.Where(a => a.WorldId == worldId && a.Timestamp >= startDate && a.Timestamp <= endDate)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<PlayerLogout>> GetPlayerLogoutEventsAsync(int worldId, DateTime startDate, DateTime? endDate)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                return await dbContext.PlayerLogoutEvents.Where(a => a.WorldId == worldId && a.Timestamp >= startDate && a.Timestamp <= endDate)
                    .ToListAsync();
            }
        }
    }
}

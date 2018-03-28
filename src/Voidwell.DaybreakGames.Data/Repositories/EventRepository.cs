using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                return await dbContext.EventDeaths.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from e in dbContext.EventDeaths

                             join weapon in dbContext.Items on e.AttackerWeaponId equals weapon.Id into weaponQ
                             from weapon in weaponQ.DefaultIfEmpty()

                             join attackerCharacter in dbContext.Characters on e.AttackerCharacterId equals attackerCharacter.Id into attackerCharacterQ
                             from attackerCharacter in attackerCharacterQ.DefaultIfEmpty()

                             join victimCharacter in dbContext.Characters on e.CharacterId equals victimCharacter.Id into victimCharacterQ
                             from victimCharacter in victimCharacterQ.DefaultIfEmpty()

                             where (e.AttackerCharacterId == characterId || e.CharacterId == characterId) && e.Timestamp > lower && e.Timestamp < upper
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

        public async Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                return await dbContext.EventFacilityControls.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                    .ToListAsync();
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

        public async Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                if (endDate == null)
                {
                    endDate = DateTime.UtcNow;
                }

                return await dbContext.EventVehicleDestroys.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                    .ToListAsync();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.ResolvedModels;

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
            using (var dbContext = _dbContextHelper.Create())
            {
                dbContext.Add<T>(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Models.Planetside.EventDeath>> GetDeathEventsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.EventDeaths.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<ResolvedModels.EventDeath>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var result = from e in dbContext.EventDeaths
                             join weapon in dbContext.Items on e.AttackerWeaponId equals weapon.Id into weaponQ
                             from weapon in weaponQ.DefaultIfEmpty()
                             join attackerCharacter in dbContext.Characters on e.AttackerCharacterId equals attackerCharacter.Id into attackerCharacterQ
                             from attackerCharacter in attackerCharacterQ.DefaultIfEmpty()
                             join victimCharacter in dbContext.Characters on e.CharacterId equals victimCharacter.Id into victimCharacterQ
                             from victimCharacter in victimCharacterQ.DefaultIfEmpty()
                             where e.AttackerCharacterId == characterId || e.CharacterId == characterId && e.Timestamp > lower && e.Timestamp < upper
                             select new ResolvedModels.EventDeath
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

                return result.ToList();
            }
        }

        public async Task<IEnumerable<EventFacilityControl>> GetFacilityControlsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.EventFacilityControls.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                    .ToListAsync();
            }
        }

        public async Task<EventFacilityControl> GetLatestFacilityControl(string worldId, string zoneId, DateTime date)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.EventFacilityControls
                    .OrderBy("Timestamp", SortDirection.Descending)
                    .FirstOrDefaultAsync(c => c.WorldId == worldId && c.ZoneId == zoneId && c.Timestamp <= date);
            }
        }

        public async Task<IEnumerable<EventVehicleDestroy>> GetVehicleDeathEventsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.EventVehicleDestroys.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                    .ToListAsync();
            }
        }
    }
}

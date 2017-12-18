using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public CharacterRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<DbCharacter> GetCharacterAsync(string characterId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Characters.FirstOrDefaultAsync(a => a.Id == characterId);
            }
        }

        public async Task<IEnumerable<DbCharacter>> GetCharactersByIdsAsync(IEnumerable<string> characterIds)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Characters.Where(a => characterIds.Contains(a.Id))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<DbCharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(string weaponItemId, string sortColumn, SortDirection sortDirection, int rowStart, int limit)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.CharacterWeaponStats.Where(s => s.ItemId == weaponItemId)
                    .Include(i => i.Character)
                    .OrderBy(sortColumn, sortDirection)
                    .Skip(rowStart)
                    .Take(limit)
                    .ToListAsync();
            }
        }

        public async Task<DbCharacter> GetCharacterWithDetailsAsync(string characterId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Characters
                    .Include(c => c.Title)
                    .Include(c => c.World)
                    .Include(c => c.Faction)
                    .Include(c => c.Time)
                    .Include(c => c.OutfitMembership)
                        .ThenInclude(m => m.Outfit)
                    .Include(c => c.Stats)
                    .Include(c => c.StatsByFaction)
                    .Include(c => c.WeaponStats)
                        .ThenInclude(s => s.Item)
                            .ThenInclude(i => i.ItemCategory)
                    .Include(c => c.WeaponStats)
                    .FirstOrDefaultAsync(c => c.Id == characterId);
            }
        }

        public async Task<DbCharacter> UpsertAsync(DbCharacter entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.Characters;

                var storeEntity = await dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.Id == entity.Id);
                if (storeEntity == null)
                {
                    dbSet.Add(entity);
                }
                else
                {
                    storeEntity = entity;
                    dbSet.Update(storeEntity);
                }

                await dbContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<DbCharacterTime> UpsertAsync(DbCharacterTime entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterTimes;

                var storeEntity = await dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.CharacterId == entity.CharacterId);
                if (storeEntity == null)
                {
                    dbSet.Add(entity);
                }
                else
                {
                    storeEntity = entity;
                    dbSet.Update(storeEntity);
                }

                await dbContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<IEnumerable<DbCharacterStat>> UpsertRangeAsync(IEnumerable<DbCharacterStat> entities)
        {
            var result = new List<DbCharacterStat>();

            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterStats;

                var storedStats = await dbSet.Where(s => entities.Any(c => c.CharacterId == s.CharacterId && c.ProfileId == s.ProfileId)).ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(a => a.ProfileId == entity.ProfileId);
                    if (storeEntity == null)
                    {
                        dbSet.Add(entity);
                        result.Add(entity);
                    }
                    else
                    {
                        foreach (var fromProp in typeof(DbCharacterStat).GetProperties())
                        {
                            var toProp = typeof(DbCharacterStat).GetProperty(fromProp.Name);
                            var toValue = toProp.GetValue(entity, null);
                            if (toValue != null)
                            {
                                fromProp.SetValue(storeEntity, toValue, null);
                            }
                        }

                        dbSet.Update(storeEntity);
                        result.Add(storeEntity);
                    }   
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IEnumerable<DbCharacterStatByFaction>> UpsertRangeAsync(IEnumerable<DbCharacterStatByFaction> entities)
        {
            var result = new List<DbCharacterStatByFaction>();

            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterStatByFactions;

                var storedStats = await dbSet.Where(s => entities.Any(c => c.CharacterId == s.CharacterId && c.ProfileId == s.ProfileId)).ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(a => a.ProfileId == entity.ProfileId);
                    if (storeEntity == null)
                    {
                        dbSet.Add(entity);
                        result.Add(entity);
                    }
                    else
                    {
                        foreach (var fromProp in typeof(DbCharacterStatByFaction).GetProperties())
                        {
                            var toProp = typeof(DbCharacterStatByFaction).GetProperty(fromProp.Name);
                            var toValue = toProp.GetValue(entity, null);
                            if (toValue != null)
                            {
                                fromProp.SetValue(storeEntity, toValue, null);
                            }
                        }

                        dbSet.Update(storeEntity);
                        result.Add(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IEnumerable<DbCharacterWeaponStat>> UpsertRangeAsync(IEnumerable<DbCharacterWeaponStat> entities)
        {
            var result = new List<DbCharacterWeaponStat>();

            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterWeaponStats;

                var storedStats = await dbSet.Where(s => entities.Any(c => c.CharacterId == s.CharacterId && c.ItemId == s.ItemId)).ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(a => a.ItemId == entity.ItemId);
                    if (storeEntity == null)
                    {
                        dbSet.Add(entity);
                        result.Add(entity);
                    }
                    else
                    {
                        foreach (var fromProp in typeof(DbCharacterWeaponStat).GetProperties())
                        {
                            var toProp = typeof(DbCharacterWeaponStat).GetProperty(fromProp.Name);
                            var toValue = toProp.GetValue(entity, null);
                            if (toValue != null)
                            {
                                fromProp.SetValue(storeEntity, toValue, null);
                            }
                        }

                        dbSet.Update(storeEntity);
                        result.Add(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IEnumerable<DbCharacterWeaponStatByFaction>> UpsertRangeAsync(IEnumerable<DbCharacterWeaponStatByFaction> entities)
        {
            var result = new List<DbCharacterWeaponStatByFaction>();

            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterWeaponStatByFactions;

                var storedStats = await dbSet.Where(s => entities.Any(c => c.CharacterId == s.CharacterId && c.ItemId == s.ItemId)).ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(a => a.ItemId == entity.ItemId);
                    if (storeEntity == null)
                    {
                        dbSet.Add(entity);
                        result.Add(entity);
                    }
                    else
                    {
                        foreach (var fromProp in typeof(DbCharacterWeaponStatByFaction).GetProperties())
                        {
                            var toProp = typeof(DbCharacterWeaponStatByFaction).GetProperty(fromProp.Name);
                            var toValue = toProp.GetValue(entity, null);
                            if (toValue != null)
                            {
                                fromProp.SetValue(storeEntity, toValue, null);
                            }
                        }

                        dbSet.Update(storeEntity);
                        result.Add(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
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

        public static T ClientMethod<T>(T element) => element;

        public async Task<DbCharacter> GetCharacterWithDetailsAsync(string characterId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var query = from c in dbContext.Characters

                             join world in dbContext.Worlds on c.WorldId equals world.Id into worldQ
                             from world in worldQ.DefaultIfEmpty()

                             join title in dbContext.Titles on c.TitleId equals title.Id into titleQ
                             from title in titleQ.DefaultIfEmpty()

                             join time in dbContext.CharacterTimes on c.Id equals time.CharacterId into timeQ
                             from time in timeQ.DefaultIfEmpty()

                             join faction in dbContext.Factions on c.FactionId equals faction.Id into factionQ
                             from faction in factionQ.DefaultIfEmpty()

                             join lifetimeStats in dbContext.CharacterLifetimeStats on c.Id equals lifetimeStats.CharacterId into lifetimeStatsQ
                             from lifetimeStats in lifetimeStatsQ.DefaultIfEmpty()

                             join lifetimeStatsByFaction in dbContext.CharacterLifetimeStatsByFaction on c.Id equals lifetimeStatsByFaction.CharacterId into lifetimeStatsByFactionQ
                             from lifetimeStatsByFaction in lifetimeStatsByFactionQ.DefaultIfEmpty()

                             join outfitMembership in (from om in dbContext.OutfitMembers
                                                       join outfit in dbContext.Outfits on om.OutfitId equals outfit.Id
                                                       where om.CharacterId == characterId
                                                       select new DbOutfitMember
                                                       {
                                                           CharacterId = om.CharacterId,
                                                           MemberSinceDate = om.MemberSinceDate,
                                                           OutfitId = om.OutfitId,
                                                           Rank = om.Rank,
                                                           RankOrdinal = om.RankOrdinal,
                                                           Outfit = outfit
                                                       }).DefaultIfEmpty() on c.Id equals outfitMembership.CharacterId

                             where c.Id == characterId
                             select new DbCharacter
                             {
                                 Id = c.Id,
                                 Name = c.Name,
                                 BattleRank = c.BattleRank,
                                 BattleRankPercentToNext = c.BattleRankPercentToNext,
                                 CertsEarned = c.CertsEarned,
                                 FactionId = c.FactionId,
                                 WorldId = c.WorldId,
                                 TitleId = c.TitleId,
                                 Time = time,
                                 World = world,
                                 Title = title,
                                 Faction = faction,
                                 OutfitMembership = outfitMembership,
                                 LifetimeStats = lifetimeStats,
                                 LifetimeStatsByFaction = lifetimeStatsByFaction,
                                 Stats = (from s in dbContext.CharacterStats
                                          join profile in dbContext.Profiles on new { pid = s.ProfileId, fid = ClientMethod(c.FactionId) } equals new { pid = profile.ProfileTypeId, fid = profile.FactionId } into profileQ
                                          from profile in profileQ.DefaultIfEmpty()
                                          where s.CharacterId == c.Id
                                          select new DbCharacterStat
                                          {
                                              CharacterId = s.CharacterId,
                                              ProfileId = s.ProfileId,
                                              Deaths = s.Deaths,
                                              FireCount = s.FireCount,
                                              HitCount = s.HitCount,
                                              KilledBy = s.KilledBy,
                                              Kills = s.Kills,
                                              PlayTime = s.PlayTime,
                                              Score = s.Score,
                                              Profile = profile
                                          }).ToList(),
                                 StatsByFaction = (from s in dbContext.CharacterStatByFactions
                                                   join profile in dbContext.Profiles on new { pid = s.ProfileId, fid = ClientMethod(c.FactionId) } equals new { pid = profile.ProfileTypeId, fid = profile.FactionId } into profileQ
                                                   from profile in profileQ.DefaultIfEmpty()
                                                   where s.CharacterId == c.Id
                                                   select new DbCharacterStatByFaction
                                                   {
                                                       CharacterId = s.CharacterId,
                                                       ProfileId = s.ProfileId,
                                                       KilledByVS = s.KilledByVS,
                                                       KilledByNC = s.KilledByNC,
                                                       KilledByTR = s.KilledByTR,
                                                       KillsVS = s.KillsVS,
                                                       KillsNC = s.KillsNC,
                                                       KillsTR = s.KillsTR,
                                                       Profile = profile
                                                   }).ToList(),
                                 WeaponStats = (from s in dbContext.CharacterWeaponStats
                                                join item in (from item in dbContext.Items
                                                              join category in dbContext.ItemCategories on item.ItemCategoryId equals category.Id into categoryQ
                                                              from category in categoryQ.DefaultIfEmpty()
                                                              select new DbItem
                                                              {
                                                                  Id = item.Id,
                                                                  Name = item.Name,
                                                                  Description = item.Description,
                                                                  FactionId = item.FactionId,
                                                                  ImageId = item.ImageId,
                                                                  IsVehicleWeapon = item != null ? item.IsVehicleWeapon : false,
                                                                  ItemTypeId = item.ItemTypeId,
                                                                  MaxStackSize = item != null ? item.MaxStackSize : 0,
                                                                  ItemCategoryId = item.ItemCategoryId,
                                                                  ItemCategory = category != null ? category : null
                                                              }) on s.ItemId equals item.Id into itemQ
                                                from item in itemQ.DefaultIfEmpty()
                                                join vehicle in dbContext.Vehicles on s.VehicleId equals vehicle.Id into vehicleQ
                                                from vehicle in vehicleQ.DefaultIfEmpty()
                                                where s.CharacterId == c.Id
                                                select new DbCharacterWeaponStat
                                                {
                                                    CharacterId = s.CharacterId,
                                                    ItemId = s.ItemId,
                                                    VehicleId = s.VehicleId,
                                                    DamageGiven = s.DamageGiven,
                                                    DamageTakenBy = s.DamageTakenBy,
                                                    Headshots = s.Headshots,
                                                    Deaths = s.Deaths,
                                                    VehicleKills = s.VehicleKills,
                                                    FireCount = s.FireCount,
                                                    HitCount = s.HitCount,
                                                    KilledBy = s.KilledBy,
                                                    Kills = s.Kills,
                                                    PlayTime = s.PlayTime,
                                                    Score = s.Score,
                                                    Item = item,
                                                    Vehicle = vehicle
                                                }).ToList(),
                                 WeaponStatsByFaction = (from s in dbContext.CharacterWeaponStatByFactions
                                                         join item in dbContext.Items on s.ItemId equals item.Id into itemQ
                                                         from item in itemQ.DefaultIfEmpty()
                                                         join vehicle in dbContext.Vehicles on s.VehicleId equals vehicle.Id into vehicleQ
                                                         from vehicle in vehicleQ.DefaultIfEmpty()
                                                         where s.CharacterId == c.Id
                                                         select new DbCharacterWeaponStatByFaction
                                                         {
                                                             CharacterId = s.CharacterId,
                                                             ItemId = s.ItemId,
                                                             VehicleId = s.VehicleId,
                                                             DamageGivenVS = s.DamageGivenVS,
                                                             DamageGivenNC = s.DamageGivenNC,
                                                             DamageGivenTR = s.DamageGivenTR,
                                                             DamageTakenByVS = s.DamageTakenByVS,
                                                             DamageTakenByNC = s.DamageTakenByNC,
                                                             DamageTakenByTR = s.DamageTakenByTR,
                                                             HeadshotsVS = s.HeadshotsVS,
                                                             HeadshotsNC = s.HeadshotsNC,
                                                             HeadshotsTR = s.HeadshotsTR,
                                                             KilledByVS = s.KilledByVS,
                                                             KilledByNC = s.KilledByNC,
                                                             KilledByTR = s.KilledByTR,
                                                             KillsVS = s.KillsVS,
                                                             KillsNC = s.KillsNC,
                                                             KillsTR = s.KillsTR,
                                                             VehicleKillsVS = s.VehicleKillsVS,
                                                             VehicleKillsNC = s.VehicleKillsNC,
                                                             VehicleKillsTR = s.VehicleKillsTR,
                                                             Item = item,
                                                             Vehicle = vehicle
                                                         }).ToList()
                             };

                return await Task.FromResult(query.ToList().FirstOrDefault());
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

        public async Task<DbCharacterLifetimeStat> UpsertAsync(DbCharacterLifetimeStat entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterLifetimeStats;

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

        public async Task<DbCharacterLifetimeStatByFaction> UpsertAsync(DbCharacterLifetimeStatByFaction entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterLifetimeStatsByFaction;

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
            var newEntities = new List<DbCharacterWeaponStat>();

            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterWeaponStats;

                var storedStats = await dbSet.Where(s => entities.Any(c => c.CharacterId == s.CharacterId && c.ItemId == s.ItemId)).ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(a => a.ItemId == entity.ItemId);
                    if (storeEntity == null)
                    {
                        newEntities.Add(entity);
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

                if (newEntities.Count() > 0)
                {
                    await dbSet.AddRangeAsync(newEntities);
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IEnumerable<DbCharacterWeaponStatByFaction>> UpsertRangeAsync(IEnumerable<DbCharacterWeaponStatByFaction> entities)
        {
            var result = new List<DbCharacterWeaponStatByFaction>();
            var newEntities = new List<DbCharacterWeaponStatByFaction>();

            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.CharacterWeaponStatByFactions;

                var storedStats = await dbSet.Where(s => entities.Any(c => c.CharacterId == s.CharacterId && c.ItemId == s.ItemId)).ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(a => a.ItemId == entity.ItemId);
                    if (storeEntity == null)
                    {
                        newEntities.Add(entity);
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

                if (newEntities.Count() > 0)
                {
                    await dbSet.AddRangeAsync(newEntities);
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }
    }
}

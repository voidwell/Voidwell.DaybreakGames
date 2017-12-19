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

        public static T ClientMethod<T>(T element) => element;

        public async Task<DbCharacter> GetCharacterWithDetailsAsync(string characterId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var result = from c in dbContext.Characters
                             join world in dbContext.Worlds on c.WorldId equals world.Id
                             join title in dbContext.Titles on c.TitleId equals title.Id
                             join time in dbContext.CharacterTimes on c.Id equals time.CharacterId
                             join faction in dbContext.Factions on c.FactionId equals faction.Id
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
                                                       }) on c.Id equals outfitMembership.CharacterId
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
                                 Stats = (from s in dbContext.CharacterStats
                                          join profile in dbContext.Profiles on new { pid = s.ProfileId, fid = ClientMethod(c.FactionId) } equals new { pid = profile.ProfileTypeId, fid = profile.FactionId } into profileQ
                                          from profile in profileQ.DefaultIfEmpty()
                                          where s.CharacterId == c.Id
                                          select new DbCharacterStat
                                          {
                                              CharacterId = s.CharacterId,
                                              ProfileId = s.ProfileId,
                                              AchievementCount = s.AchievementCount,
                                              AssistCount = s.AssistCount,
                                              DominationCount = s.DominationCount,
                                              Deaths = s.Deaths,
                                              FacilityCaptureCount = s.FacilityCaptureCount,
                                              FacilityDefendedCount = s.FacilityDefendedCount,
                                              FireCount = s.FireCount,
                                              HitCount = s.HitCount,
                                              KilledBy = s.KilledBy,
                                              Kills = s.Kills,
                                              MedalCount = s.MedalCount,
                                              PlayTime = s.PlayTime,
                                              RevengeCount = s.RevengeCount,
                                              Score = s.Score,
                                              SkillPoints = s.SkillPoints,
                                              WeaponDamageGiven = s.WeaponDamageGiven,
                                              WeaponDamageTakenBy = s.WeaponDamageTakenBy,
                                              WeaponDeaths = s.WeaponDeaths,
                                              WeaponFireCount = s.WeaponFireCount,
                                              WeaponHeadshots = s.WeaponHeadshots,
                                              WeaponHitCount = s.WeaponHitCount,
                                              WeaponKilledBy = s.WeaponKilledBy,
                                              WeaponKills = s.WeaponKills,
                                              WeaponPlayTime = s.WeaponPlayTime,
                                              WeaponScore = s.WeaponScore,
                                              WeaponVehicleKills = s.WeaponVehicleKills,
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
                                                       DominationCountVS = s.DominationCountVS,
                                                       DominationCountNC = s.DominationCountNC,
                                                       DominationCountTR = s.DominationCountTR,
                                                       FacilityCaptureCountVS = s.FacilityCaptureCountVS,
                                                       FacilityCaptureCountNC = s.FacilityCaptureCountNC,
                                                       FacilityCaptureCountTR = s.FacilityCaptureCountTR,
                                                       KilledByVS = s.KilledByVS,
                                                       KilledByNC = s.KilledByNC,
                                                       KilledByTR = s.KilledByTR,
                                                       KillsVS = s.KillsVS,
                                                       KillsNC = s.KillsNC,
                                                       KillsTR = s.KillsTR,
                                                       RevengeCountVS = s.RevengeCountVS,
                                                       RevengeCountNC = s.RevengeCountNC,
                                                       RevengeCountTR = s.RevengeCountTR,
                                                       WeaponDamageGivenVS = s.WeaponDamageGivenVS,
                                                       WeaponDamageGivenNC = s.WeaponDamageGivenNC,
                                                       WeaponDamageGivenTR = s.WeaponDamageGivenTR,
                                                       WeaponDamageTakenByVS = s.WeaponDamageTakenByVS,
                                                       WeaponDamageTakenByNC = s.WeaponDamageTakenByNC,
                                                       WeaponDamageTakenByTR = s.WeaponDamageTakenByTR,
                                                       WeaponHeadshotsVS = s.WeaponHeadshotsVS,
                                                       WeaponHeadshotsNC = s.WeaponHeadshotsNC,
                                                       WeaponHeadshotsTR = s.WeaponHeadshotsTR,
                                                       WeaponKilledByVS = s.WeaponKilledByVS,
                                                       WeaponKilledByNC = s.WeaponKilledByNC,
                                                       WeaponKilledByTR = s.WeaponKilledByTR,
                                                       WeaponKillsVS = s.WeaponKillsVS,
                                                       WeaponKillsNC = s.WeaponKillsNC,
                                                       WeaponKillsTR = s.WeaponKillsTR,
                                                       WeaponVehicleKillsVS = s.WeaponVehicleKillsVS,
                                                       WeaponVehicleKillsNC = s.WeaponVehicleKillsNC,
                                                       WeaponVehicleKillsTR = s.WeaponVehicleKillsTR,
                                                       Profile = profile
                                                   }).ToList(),
                                 WeaponStats = (from s in dbContext.CharacterWeaponStats
                                                join item in dbContext.Items on s.ItemId equals item.Id into itemQ
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
                                                         }).ToList(),
                             };

                return result.ToList().FirstOrDefault();
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

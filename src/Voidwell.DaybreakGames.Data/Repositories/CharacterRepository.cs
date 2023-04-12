using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Microservice.EntityFramework;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public CharacterRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<string> GetCharacterIdByName(string characterName)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var character = await dbContext.Characters
                    .Include(a => a.Time)
                    .Where(a => a.Time != null && a.Name != null)
                    .OrderByDescending(a => a.Time.LastLoginDate)
                    .FirstOrDefaultAsync(a => a.Name.ToLower() == characterName.ToLower());

                return character?.Id;
            }
        }

        public async Task<Character> GetCharacterAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Characters
                    .Include(a => a.Time)
                    .FirstOrDefaultAsync(a => a.Id == characterId);
            }
        }

        public async Task<IEnumerable<Character>> GetCharactersByIdsAsync(IEnumerable<string> characterIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Characters.Where(a => characterIds.Contains(a.Id))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<CharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page, int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterWeaponStats.Where(s => s.ItemId == weaponItemId && s.Kills > 1)
                    .Include(i => i.Character)
                    .OrderByDescending(a => a.Kills)
                    .Skip(page * limit)
                    .Take(limit)
                    .ToListAsync();
            }
        }

        public async Task<Character> GetCharacterWithDetailsAsync(string characterId)
        {
            var results = await GetCharacterWithDetailsAsync(new[] { characterId });
            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<Character>> GetCharacterWithDetailsAsync(params string[] characterIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

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

                            join outfitMembership in dbContext.OutfitMembers on c.Id equals outfitMembership.CharacterId into outfitMemberships
                            from outfitMembership in outfitMemberships.DefaultIfEmpty()

                            join outfit in dbContext.Outfits on outfitMembership.OutfitId equals outfit.Id into outfits
                            from outfit in outfits.DefaultIfEmpty()

                            where characterIds.Contains(c.Id)
                            select new Character
                            {
                                Id = c.Id,
                                Name = c.Name,
                                BattleRank = c.BattleRank,
                                BattleRankPercentToNext = c.BattleRankPercentToNext,
                                PrestigeLevel = c.PrestigeLevel,
                                CertsEarned = c.CertsEarned,
                                FactionId = c.FactionId,
                                WorldId = c.WorldId,
                                TitleId = c.TitleId,
                                Time = time,
                                World = world,
                                Title = title,
                                Faction = faction,
                                LifetimeStats = lifetimeStats,
                                LifetimeStatsByFaction = lifetimeStatsByFaction,
                                OutfitMembership = outfitMembership == null ? null : new OutfitMember
                                {
                                    CharacterId = outfitMembership.CharacterId,
                                    MemberSinceDate = outfitMembership.MemberSinceDate,
                                    OutfitId = outfitMembership.OutfitId,
                                    Rank = outfitMembership.Rank,
                                    RankOrdinal = outfitMembership.RankOrdinal,
                                    Outfit = outfit
                                }
                            };
                var details = await query.ToListAsync();

                foreach(var c in details)
                {
                    c.Stats = await (from s in dbContext.CharacterStats
                                     join profile in dbContext.Profiles on new { pid = s.ProfileId, fid = c.FactionId } equals new { pid = profile.ProfileTypeId, fid = profile.FactionId } into profileQ
                                     from profile in profileQ.DefaultIfEmpty()

                                     where s.CharacterId == c.Id
                                     select new CharacterStat
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
                                     }).ToListAsync();

                    c.StatsHistory = await (from s in dbContext.CharacterStatHistory
                                            where s.CharacterId == c.Id
                                            select s).ToListAsync();

                    c.WeaponStats = await (from s in dbContext.CharacterWeaponStats
                                           join item in dbContext.Items on s.ItemId equals item.Id into items
                                           from item in items.DefaultIfEmpty()

                                           join itemCategory in dbContext.ItemCategories on item.ItemCategoryId equals itemCategory.Id into itemCategories
                                           from itemCategory in itemCategories.DefaultIfEmpty()

                                           join vehicle in dbContext.Vehicles on s.VehicleId equals vehicle.Id into vehicleQ
                                           from vehicle in vehicleQ.DefaultIfEmpty()

                                           where s.CharacterId == c.Id
                                           select new CharacterWeaponStat
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
                                               Item = item == null ? null : new Item
                                               {
                                                   Id = item.Id,
                                                   Name = item.Name,
                                                   Description = item.Description,
                                                   FactionId = item.FactionId,
                                                   ImageId = item.ImageId,
                                                   IsVehicleWeapon = item.IsVehicleWeapon,
                                                   ItemTypeId = item.ItemTypeId,
                                                   MaxStackSize = item.MaxStackSize,
                                                   ItemCategoryId = item.ItemCategoryId,
                                                   ItemCategory = itemCategory,
                                               },
                                               Vehicle = vehicle
                                           }).ToListAsync();

                    c.WeaponStatsByFaction = await (from s in dbContext.CharacterWeaponStatByFactions
                                                    join item in dbContext.Items on s.ItemId equals item.Id into itemQ
                                                    from item in itemQ.DefaultIfEmpty()

                                                    join vehicle in dbContext.Vehicles on s.VehicleId equals vehicle.Id into vehicleQ
                                                    from vehicle in vehicleQ.DefaultIfEmpty()

                                                    where s.CharacterId == c.Id
                                                    select new CharacterWeaponStatByFaction
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
                                                    }).ToListAsync();
                }

                return details;
            }
        }

        public async Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterWeaponStats.Where(a => a.CharacterId == characterId).ToListAsync();
            }
        }

        public async Task<CharacterRating> GetCharacterRatingAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterRating.FirstOrDefaultAsync(a => a.CharacterId == characterId);
            }
        }

        public async Task<IEnumerable<CharacterRating>> GetCharacterRatingLeaderboardAsync(int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterRating
                    .Include(i => i.Character)
                    .Take(limit)
                    .OrderByDescending(a => a.Rating)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from characterAch in dbContext.CharacterAchievements
                            join achievement in dbContext.Achievements on characterAch.AchievementId equals achievement.Id into achievements
                            from achievement in achievements.DefaultIfEmpty()
                            where characterAch.CharacterId == characterId
                            select new CharacterAchievement
                            {
                                CharacterId = characterAch.CharacterId,
                                AchievementId = characterAch.AchievementId,
                                EarnedCount = characterAch.EarnedCount,
                                StartDate = characterAch.StartDate,
                                FinishDate = characterAch.FinishDate,
                                Achievement = achievement
                            };
                return await query.ToListAsync();
            }
        }

        public async Task<Character> UpsertAsync(Character entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertAsync(entity);
            }
        }

        public async Task<CharacterTime> UpsertAsync(CharacterTime entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertAsync(entity);
            }
        }

        public async Task<CharacterLifetimeStat> UpsertAsync(CharacterLifetimeStat entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertWithoutNullPropertiesAsync(entity);
            }
        }

        public async Task<CharacterLifetimeStatByFaction> UpsertAsync(CharacterLifetimeStatByFaction entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertWithoutNullPropertiesAsync(entity);
            }
        }

        public async Task<CharacterRating> UpsertAsync(CharacterRating entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertAsync(entity);
            }
        }

        public async Task<IEnumerable<CharacterStat>> UpsertRangeAsync(IEnumerable<CharacterStat> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertRangeWithoutNullPropertiesAsync(entities);
            }
        }

        public async Task<IEnumerable<CharacterStatByFaction>> UpsertRangeAsync(IEnumerable<CharacterStatByFaction> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertRangeWithoutNullPropertiesAsync(entities);
            }
        }

        public async Task<IEnumerable<CharacterWeaponStat>> UpsertRangeAsync(IEnumerable<CharacterWeaponStat> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertRangeWithoutNullPropertiesAsync(entities);
            }
        }

        public async Task<IEnumerable<CharacterWeaponStatByFaction>> UpsertRangeAsync(IEnumerable<CharacterWeaponStatByFaction> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertRangeWithoutNullPropertiesAsync(entities);
            }
        }

        public async Task<IEnumerable<CharacterStatHistory>> UpsertRangeAsync(IEnumerable<CharacterStatHistory> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertAsync(entities);
            }
        }

        public async Task<IEnumerable<CharacterAchievement>> UpsertRangeAsync(IEnumerable<CharacterAchievement> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpsertAsync(entities);
            }
        }
    }
}

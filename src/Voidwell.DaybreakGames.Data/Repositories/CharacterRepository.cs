using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class CharacterRepository: Repository, ICharacterRepository
    {
        public static T ClientMethod<T>(T element) => element;
        private readonly IDbContextHelper _dbContextHelper;

        public CharacterRepository(IDbContextHelper dbContextHelper) : base(dbContextHelper)
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
                    .Where(a => a.Time != null)
                    .OrderByDescending(a => a.Time.CreatedDate)
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

        public async Task<IEnumerable<CharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, string sortColumn, SortDirection sortDirection, int rowStart, int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterWeaponStats.Where(s => s.ItemId == weaponItemId && s.Kills > 1)
                    .Include(i => i.Character)
                    .OrderBy(sortColumn, sortDirection)
                    .Skip(rowStart)
                    .Take(limit)
                    .ToListAsync();
            }
        }

        public async Task<Character> GetCharacterWithDetailsAsync(string characterId)
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

                            where c.Id == characterId
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
                                 OutfitMembership = (from om in dbContext.OutfitMembers
                                                     join outfit in dbContext.Outfits on om.OutfitId equals outfit.Id
                                                     where om.CharacterId == characterId
                                                     select new OutfitMember
                                                     {
                                                         CharacterId = om.CharacterId,
                                                         MemberSinceDate = om.MemberSinceDate,
                                                         OutfitId = om.OutfitId,
                                                         Rank = om.Rank,
                                                         RankOrdinal = om.RankOrdinal,
                                                         Outfit = outfit
                                                     }).FirstOrDefault(),
                                 Stats = (from s in dbContext.CharacterStats
                                          join profile in dbContext.Profiles on new { pid = s.ProfileId, fid = ClientMethod(c.FactionId) } equals new { pid = profile.ProfileTypeId, fid = profile.FactionId } into profileQ
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
                                          }).ToList(),
                                 StatsHistory = (from s in dbContext.CharacterStatHistory
                                          where s.CharacterId == c.Id
                                          select s).ToList(),
                                 StatsByFaction = (from s in dbContext.CharacterStatByFactions
                                                   join profile in dbContext.Profiles on new { pid = s.ProfileId, fid = ClientMethod(c.FactionId) } equals new { pid = profile.ProfileTypeId, fid = profile.FactionId } into profileQ
                                                   from profile in profileQ.DefaultIfEmpty()
                                                   where s.CharacterId == c.Id
                                                   select new CharacterStatByFaction
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
                                                              select new Item
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
                                                                  ItemCategory = category
                                                              }) on s.ItemId equals item.Id into itemQ
                                                from item in itemQ.DefaultIfEmpty()
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
                                                    Item = item,
                                                    Vehicle = vehicle
                                                }).ToList(),
                                 WeaponStatsByFaction = (from s in dbContext.CharacterWeaponStatByFactions
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
                                                         }).ToList()
                             };
                var result = query.FirstOrDefault();

                return await Task.FromResult(result);
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

        public Task<Character> UpsertAsync(Character entity)
        {
            return UpsertAsync(entity, a => a.Id == entity.Id);
        }

        public Task<CharacterTime> UpsertAsync(CharacterTime entity)
        {
            return UpsertAsync(entity, a => a.CharacterId == entity.CharacterId);
        }

        public Task<CharacterLifetimeStat> UpsertAsync(CharacterLifetimeStat entity)
        {
            return UpsertAsync(entity, a => a.CharacterId == entity.CharacterId, true);
        }

        public Task<CharacterLifetimeStatByFaction> UpsertAsync(CharacterLifetimeStatByFaction entity)
        {
            return UpsertAsync(entity, a => a.CharacterId == entity.CharacterId, true);
        }

        public Task<CharacterRating> UpsertAsync(CharacterRating entity)
        {
            return UpsertAsync(entity, a => a.CharacterId == entity.CharacterId);
        }

        public Task<IEnumerable<CharacterStat>> UpsertRangeAsync(IEnumerable<CharacterStat> entities)
        {
            var id = entities.FirstOrDefault()?.CharacterId;
            return UpsertRangeAsync(entities, s => s.CharacterId == id, (a, b) => a.ProfileId == b.ProfileId, true);
        }

        public Task<IEnumerable<CharacterStatByFaction>> UpsertRangeAsync(IEnumerable<CharacterStatByFaction> entities)
        {
            var id = entities.FirstOrDefault()?.CharacterId;
            return UpsertRangeAsync(entities, s => s.CharacterId == id, (a, b) => a.ProfileId == b.ProfileId, true);
        }

        public Task<IEnumerable<CharacterWeaponStat>> UpsertRangeAsync(IEnumerable<CharacterWeaponStat> entities)
        {
            var id = entities.FirstOrDefault()?.CharacterId;
            return UpsertRangeAsync(entities, s => s.CharacterId == id, (a, b) => a.ItemId == b.ItemId && a.VehicleId == b.VehicleId, true);
        }

        public Task<IEnumerable<CharacterWeaponStatByFaction>> UpsertRangeAsync(IEnumerable<CharacterWeaponStatByFaction> entities)
        {
            var id = entities.FirstOrDefault()?.CharacterId;
            return UpsertRangeAsync(entities, s => s.CharacterId == id, (a, b) => a.ItemId == b.ItemId && a.VehicleId == b.VehicleId, true);
        }

        public Task<IEnumerable<CharacterStatHistory>> UpsertRangeAsync(IEnumerable<CharacterStatHistory> entities)
        {
            var id = entities.FirstOrDefault()?.CharacterId;
            return UpsertRangeAsync(entities, s => s.CharacterId == id, (a, b) => a.StatName == b.StatName);
        }
    }
}

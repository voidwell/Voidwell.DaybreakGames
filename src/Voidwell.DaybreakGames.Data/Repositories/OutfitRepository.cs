using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class OutfitRepository : IOutfitRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public OutfitRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<string> GetOutfitIdByAlias(string outfitAlias)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var outfit = await dbContext.Outfits.FirstOrDefaultAsync(a => a.Alias == outfitAlias);

                return outfit?.Id;
            }
        }

        public async Task<Outfit> GetOutfitAsync(string outfitId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Outfits.FirstOrDefaultAsync(a => a.Id == outfitId);
            }
        }

        public async Task<Outfit> GetOutfitDetailsAsync(string outfitId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from outfit in dbContext.Outfits

                             join world in dbContext.Worlds on outfit.WorldId equals world.Id into worldQ
                             from world in worldQ.DefaultIfEmpty()

                             join faction in dbContext.Factions on outfit.FactionId equals faction.Id into factionQ
                             from faction in factionQ.DefaultIfEmpty()

                             join leader in dbContext.Characters on outfit.LeaderCharacterId equals leader.Id into leaderQ
                             from leader in leaderQ.DefaultIfEmpty()

                             where outfit.Id == outfitId
                             select new { outfit, world, faction, leader };

                return (await query.ToListAsync()).Select(a =>
                {
                    var outfit = a.outfit;

                    outfit.World = a.world;
                    outfit.Faction = a.faction;
                    outfit.LeaderCharacter = a.leader;

                    return outfit;
                }).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<OutfitMember>> GetOutfitMembersAsync(string outfitId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.OutfitMembers.Where(a => a.OutfitId == outfitId)
                    .Include(a => a.Character)
                        .ThenInclude(a => a.Time)
                    .Include(a => a.Character)
                        .ThenInclude(a => a.LifetimeStats)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Outfits.Where(a => outfitIds.Contains(a.Id))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Outfit>> GetOutfitsByNameAsync(string name, int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Outfits.Where(o => o.Name.Contains(name))
                    .Take(12)
                    .ToListAsync();
            }
        }

        public async Task RemoveOutfitMemberAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbMembership = await dbContext.OutfitMembers.FindAsync(characterId);
                if (dbMembership != null)
                {
                    dbContext.OutfitMembers.Remove(dbMembership);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<OutfitMember> UpsertAsync(OutfitMember entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.OutfitMembers;

                var storeEntity = await dbContext.OutfitMembers.AsNoTracking().FirstOrDefaultAsync(a => a.CharacterId == entity.CharacterId);
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

        public async Task<Outfit> UpsertAsync(Outfit entity)
        {
            Outfit result = null;

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.Outfits;

                var storeEntity = await dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.Id == entity.Id);
                if (storeEntity == null)
                {
                    dbSet.Add(entity);
                    result = entity;
                }
                else
                {
                    storeEntity = entity;
                    dbSet.Update(storeEntity);
                    result = storeEntity;
                }

                await dbContext.SaveChangesAsync();
            }

            return result;
        }
    }
}

using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class OutfitService : IOutfitService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusOutfit _censusOutfit;
        private readonly CensusCharacter _censusCharacter;

        public OutfitService(PS2DbContext ps2DbContext, CensusOutfit censusOutfit, CensusCharacter censusCharacter)
        {
            _ps2DbContext = ps2DbContext;
            _censusOutfit = censusOutfit;
            _censusCharacter = censusCharacter;
        }

        public async Task<IEnumerable<DbOutfit>> FindOutfits(IEnumerable<string> outfitIds)
        {
            return await _ps2DbContext.Outfits.Where(o => outfitIds.Contains(o.Id))
                .ToListAsync();
        }

        public async Task<DbOutfit> GetOutfit(string outfitId)
        {
            var outfit = _ps2DbContext.Outfits.Where(o => o.Id == outfitId)
                .FirstOrDefault();

            if (outfit != null)
            {
                return outfit;
            }

            return await UpdateOutfit(outfitId);
        }

        public async Task<DbOutfit> GetOutfitFull(string outfitId)
        {
            var outfit = TryGetOutfitFull(outfitId);

            if (outfit != null)
            {
                return outfit;
            }

            await UpdateOutfit(outfitId);

            return TryGetOutfitFull(outfitId);
        }

        public IEnumerable<DbOutfitMember> GetOutfitMembers(string outfitId)
        {
            return _ps2DbContext.OutfitMembers.Where(o => o.OutfitId == outfitId)
                .ToList();
        }

        public async Task<IEnumerable<DbOutfit>> LookupOutfitsByName(string name, int limit = 12)
        {
            return await _ps2DbContext.Outfits.Where(o => o.Name.Contains(name))
                .Take(12)
                .ToListAsync();
        }

        public async Task<DbOutfit> UpdateOutfit(string outfitId)
        {
            var outfit = await _censusOutfit.GetOutfit(outfitId);

            if (outfit == null)
            {
                return null;
            }

            var leader = await _censusCharacter.GetCharacter(outfit.LeaderCharacterId);

            var dataModel = new DbOutfit
            {
                Id = outfit.OutfitId,
                Name = outfit.Name,
                Alias = outfit.Alias,
                CreatedDate = outfit.TimeCreated,
                LeaderCharacterId = outfit.LeaderCharacterId,
                MemberCount = outfit.MemberCount,
                FactionId = leader.FactionId,
                WorldId = leader.WorldId
            };

            _ps2DbContext.Outfits.Update(dataModel);
            await _ps2DbContext.SaveChangesAsync();

            return dataModel;
        }

        private DbOutfit TryGetOutfitFull(string outfitId)
        {
            return _ps2DbContext.Outfits.Where(o => o.Id == outfitId)
                .Include(i => i.World)
                .Include(i => i.Faction)
                .Include(i => i.Leader)
                .FirstOrDefault();
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}

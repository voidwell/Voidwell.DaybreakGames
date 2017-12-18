using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class OutfitService : IOutfitService
    {
        private readonly IOutfitRepository _outfitRepository;
        private readonly CensusOutfit _censusOutfit;
        private readonly CensusCharacter _censusCharacter;
        private readonly ILogger<OutfitService> _logger;

        public OutfitService(IOutfitRepository outfitRepository, CensusOutfit censusOutfit, CensusCharacter censusCharacter, ILogger<OutfitService> logger)
        {
            _outfitRepository = outfitRepository;
            _censusOutfit = censusOutfit;
            _censusCharacter = censusCharacter;
            _logger = logger;
        }

        public Task<IEnumerable<DbOutfit>> FindOutfits(IEnumerable<string> outfitIds)
        {
            return _outfitRepository.GetOutfitsByIdsAsync(outfitIds);
        }

        public async Task<DbOutfit> GetOutfit(string outfitId)
        {
            var outfit = await _outfitRepository.GetOutfitAsync(outfitId);
            if (outfit != null)
            {
                return outfit;
            }

            return await UpdateOutfit(outfitId);
        }

        public async Task<DbOutfit> GetOutfitFull(string outfitId)
        {
            var outfit = await TryGetOutfitFull(outfitId);

            if (outfit != null)
            {
                return outfit;
            }

            await UpdateOutfit(outfitId);

            return await TryGetOutfitFull(outfitId);
        }

        public Task<IEnumerable<DbOutfitMember>> GetOutfitMembers(string outfitId)
        {
            return _outfitRepository.GetOutfitMembersAsync(outfitId);
        }

        public Task<IEnumerable<DbOutfit>> LookupOutfitsByName(string name, int limit = 12)
        {
            return _outfitRepository.GetOutfitsByNameAsync(name, limit);
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

            return await _outfitRepository.UpsertAsync(dataModel);
        }

        public async Task<DbOutfitMember> UpdateCharacterOutfitMembership(string characterId)
        {
            var membership = await _censusCharacter.GetCharacterOutfitMembership(characterId);

            if (membership == null)
            {
                await _outfitRepository.RemoveOutfitMemberAsync(characterId);
                return null;
            }

            await GetOutfit(membership.OutfitId);

            var dataModel = new DbOutfitMember
            {
                OutfitId = membership.OutfitId,
                CharacterId = membership.CharacterId,
                MemberSinceDate = membership.MemberSinceDate,
                Rank = membership.Rank,
                RankOrdinal = membership.RankOrdinal
            };

            return await _outfitRepository.UpsertAsync(dataModel);
        }

        private Task<DbOutfit> TryGetOutfitFull(string outfitId)
        {
            return _outfitRepository.GetOutfitDetailsAsync(outfitId);
        }
    }
}

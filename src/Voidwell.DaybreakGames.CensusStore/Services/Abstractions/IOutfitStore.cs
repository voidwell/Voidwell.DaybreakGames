using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IOutfitStore
    {
        Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds);
        Task<Outfit> GetOutfitDetailsAsync(string outfitId);
        Task<string> GetOutfitIdByAliasAsync(string outfitAlias);
        Task<IEnumerable<OutfitMember>> GetOutfitMembersAsync(string outfitId);
        Task<IEnumerable<Outfit>> GetOutfitsByNameAsync(string name, int limit = 12);
        Task<Outfit> GetOutfitByAliasAsync(string alias);
        Task<OutfitMember> UpdateCharacterOutfitMembershipAsync(Character character);
        Task<Outfit> GetOutfitAsync(string outfitId, Character member = null);
    }
}
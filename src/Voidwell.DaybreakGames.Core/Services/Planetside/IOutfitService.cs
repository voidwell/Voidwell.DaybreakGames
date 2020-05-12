using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IOutfitService
    {
        Task<IEnumerable<Outfit>> LookupOutfitsByName(string name, int limit = 12);
        Task<Outfit> LookupOutfitByAlias(string alias);
        Task<Outfit> GetOutfit(string outfitId);
        Task<OutfitDetails> GetOutfitDetails(string outfitId);
        Task<Outfit> GetOutfitDetailsAsync(string outfitId);
        Task<IEnumerable<OutfitMemberDetails>> GetOutfitMembers(string outfitId);
        Task<IEnumerable<Outfit>> FindOutfits(IEnumerable<string> outfitIds);
        Task<OutfitMember> UpdateCharacterOutfitMembership(Character character);
        Task<OutfitDetails> GetOutfitByAlias(string outfitAlias);
    }
}

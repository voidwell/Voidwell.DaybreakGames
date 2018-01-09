using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IOutfitService
    {
        Task<IEnumerable<DbOutfit>> LookupOutfitsByName(string name, int limit = 12);
        Task<DbOutfit> GetOutfit(string outfitId);
        Task<OutfitDetails> GetOutfitDetails(string outfitId);
        Task<DbOutfit> GetOutfitDetailsAsync(string outfitId);
        Task<IEnumerable<OutfitMemberDetails>> GetOutfitMembers(string outfitId);
        Task<DbOutfit> UpdateOutfit(string outfitId);
        Task<IEnumerable<DbOutfit>> FindOutfits(IEnumerable<string> outfitIds);
        Task<DbOutfitMember> UpdateCharacterOutfitMembership(string characterId);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IOutfitService
    {
        Task<IEnumerable<Outfit>> LookupOutfitsByName(string name, int limit = 12);
        Task<Outfit> LookupOutfitByAlias(string alias);
        Task<OutfitDetails> GetOutfitDetails(string outfitId);
        Task<IEnumerable<OutfitMemberDetails>> GetOutfitMembers(string outfitId);
        Task<IEnumerable<Outfit>> FindOutfits(IEnumerable<string> outfitIds);
        Task<OutfitDetails> GetOutfitByAlias(string outfitAlias);
    }
}

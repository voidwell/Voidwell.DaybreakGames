using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IOutfitService
    {
        Task<IEnumerable<DbOutfit>> LookupOutfitsByName(string name, int limit = 12);
        Task<DbOutfit> GetOutfit(string outfitId);
        Task<DbOutfit> GetOutfitFull(string outfitId);
        Task<IEnumerable<DbOutfitMember>> GetOutfitMembers(string outfitId);
        Task<DbOutfit> UpdateOutfit(string outfitId);
        Task<IEnumerable<DbOutfit>> FindOutfits(IEnumerable<string> outfitIds);
    }
}

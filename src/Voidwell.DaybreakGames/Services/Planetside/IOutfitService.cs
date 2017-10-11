using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IOutfitService
    {
        IEnumerable<DbOutfit> LookupOutfitsByName(string name, int limit = 12);
        Task<DbOutfit> GetOutfit(string outfitId);
        Task<DbOutfit> GetOutfitFull(string outfitId);
        IEnumerable<DbOutfitMember> GetOutfitMembers(string outfitId);
        Task<DbOutfit> UpdateOutfit(string outfitId);
        IEnumerable<DbOutfit> FindOutfits(params string[] outfitIds);
    }
}

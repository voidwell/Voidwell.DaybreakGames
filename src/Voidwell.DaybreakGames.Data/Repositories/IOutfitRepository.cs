using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IOutfitRepository
    {
        Task<IEnumerable<DbOutfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds);
        Task<DbOutfit> GetOutfitAsync(string outfitId);
        Task<DbOutfit> GetOutfitDetailsAsync(string outfitId);
        Task<IEnumerable<DbOutfitMember>> GetOutfitMembersAsync(string outfitId);
        Task<IEnumerable<DbOutfit>> GetOutfitsByNameAsync(string name, int limit);
        Task RemoveOutfitMemberAsync(string characterId);
        Task<DbOutfitMember> UpsertAsync(DbOutfitMember entity);
        Task<DbOutfit> UpsertAsync(DbOutfit entity);
    }
}
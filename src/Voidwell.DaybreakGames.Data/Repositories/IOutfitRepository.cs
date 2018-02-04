using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IOutfitRepository
    {
        Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds);
        Task<Outfit> GetOutfitAsync(string outfitId);
        Task<Outfit> GetOutfitDetailsAsync(string outfitId);
        Task<IEnumerable<OutfitMember>> GetOutfitMembersAsync(string outfitId);
        Task<IEnumerable<Outfit>> GetOutfitsByNameAsync(string name, int limit);
        Task RemoveOutfitMemberAsync(string characterId);
        Task<OutfitMember> UpsertAsync(OutfitMember entity);
        Task<Outfit> UpsertAsync(Outfit entity);
    }
}
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions;

public interface IOutfitRepository
{
    Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<long> outfitIds);
    Task<Outfit?> GetOutfitAsync(long outfitId);
    Task<Outfit?> GetOutfitDetailsAsync(long outfitId);
    Task<IEnumerable<OutfitMember>> GetOutfitMembersAsync(long outfitId);
    Task<IEnumerable<Outfit>> GetOutfitsByNameAsync(string name, int limit, IEnumerable<int>? worldIds = null);
    Task<Outfit?> GetOutfitByAliasAsync(string alias, IEnumerable<int>? worldIds = null);
    Task RemoveOutfitMemberAsync(long characterId);
    Task<OutfitMember> UpsertAsync(OutfitMember entity);
    Task<Outfit> UpsertAsync(Outfit entity);
    Task<long?> GetOutfitIdByAlias(string outfitAlias, IEnumerable<int>? worldIds = null);
}

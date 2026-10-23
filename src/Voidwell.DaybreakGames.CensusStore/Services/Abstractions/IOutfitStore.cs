using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface IOutfitStore
{
    Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<long> outfitIds);
    Task<Outfit?> GetOutfitDetailsAsync(long outfitId);
    Task<long?> GetOutfitIdByAliasAsync(string outfitAlias);
    Task<IEnumerable<OutfitMember>> GetOutfitMembersAsync(long outfitId);
    Task<IEnumerable<Outfit>> GetOutfitsByNameAsync(string name, int limit = 12);
    Task<Outfit> GetOutfitByAliasAsync(string alias);
    Task<OutfitMember?> UpdateCharacterOutfitMembershipAsync(Character character);
    Task<Outfit?> GetOutfitAsync(long outfitId, Character? member = null);
}

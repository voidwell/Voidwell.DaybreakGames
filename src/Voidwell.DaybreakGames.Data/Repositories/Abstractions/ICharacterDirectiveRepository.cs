using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions;

public interface ICharacterDirectiveRepository
{
    Task<IEnumerable<CharacterDirectiveTree>> GetDirectiveTreesAsync(long characterId);
    Task<IEnumerable<CharacterDirectiveTier>> GetDirectiveTiersAsync(long characterId);
    Task<IEnumerable<CharacterDirective>> GetDirectivesAsync(long characterId);
    Task<IEnumerable<CharacterDirectiveObjective>> GetDirectiveObjectivesAsync(long characterId);
}

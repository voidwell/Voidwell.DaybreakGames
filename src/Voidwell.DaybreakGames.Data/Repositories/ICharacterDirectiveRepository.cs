using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ICharacterDirectiveRepository
    {
        Task<IEnumerable<CharacterDirectiveTree>> GetDirectiveTreesAsync(string characterId);
        Task<IEnumerable<CharacterDirectiveTier>> GetDirectiveTiersAsync(string characterId);
        Task<IEnumerable<CharacterDirective>> GetDirectivesAsync(string characterId);
        Task<IEnumerable<CharacterDirectiveObjective>> GetDirectiveObjectivesAsync(string characterId);
    }
}
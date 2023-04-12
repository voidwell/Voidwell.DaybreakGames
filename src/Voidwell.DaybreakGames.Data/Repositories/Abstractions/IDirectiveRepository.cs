using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IDirectiveRepository
    {
        Task<IEnumerable<DirectiveTreeCategory>> GetDirectiveTreesCategoriesAsync();
        Task<IEnumerable<DirectiveTree>> GetDirectiveTreesAsync();
        Task<IEnumerable<DirectiveTier>> GetDirectiveTiersAsync();
        Task<IEnumerable<Directive>> GetDirectivesAsync();
    }
}
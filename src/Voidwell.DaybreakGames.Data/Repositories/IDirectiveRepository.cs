using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IDirectiveRepository
    {
        Task<IEnumerable<DirectiveTreeCategory>> GetDirectiveTreesCategoriesAsync();
        Task<IEnumerable<Directive>> GetDirectivesAsync();
    }
}
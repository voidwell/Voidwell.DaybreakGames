using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> SearchPlanetside(string category, string query);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> SearchPlanetside(string query);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> SearchPlanetside(string category, string query);
    }
}

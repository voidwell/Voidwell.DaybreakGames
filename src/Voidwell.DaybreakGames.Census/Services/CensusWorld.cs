using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Services
{
    public class CensusWorld
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusWorld(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusWorldModel>> GetAllWorlds()
        {
            var query = _queryFactory.Create("world");
            query.SetLanguage("en");

            return await query.GetBatchAsync<CensusWorldModel>();
        }
    }
}

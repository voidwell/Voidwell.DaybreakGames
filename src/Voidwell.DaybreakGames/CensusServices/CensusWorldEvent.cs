using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusWorldEvent
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusWorldEvent(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusMetagameWorldEventModel>> GetMetagameWorldEvents()
        {
            var query = _queryFactory.Create("world_event");
            query.Where("type").Equals("METAGAME");

            return await query.GetListAsync<CensusMetagameWorldEventModel>();
        }
    }
}

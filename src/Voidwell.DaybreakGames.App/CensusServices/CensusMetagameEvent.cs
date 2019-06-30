using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusMetagameEvent
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusMetagameEvent(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusMetagameEventCategoryModel>> GetAllCategories()
        {
            var query = _queryFactory.Create("metagame_event");
            query.SetLanguage("en");

            query.ShowFields("metagame_event_id", "name", "description", "type", "experience_bonus");

            return await query.GetBatchAsync<CensusMetagameEventCategoryModel>();
        }

        public async Task<IEnumerable<CensusMetagameEventStateModel>> GetAllStates()
        {
            var query = _queryFactory.Create("metagame_event_state");
            query.SetLanguage("en");

            query.ShowFields("metagame_event_state_id", "name");

            return await query.GetBatchAsync<CensusMetagameEventStateModel>();
        }
    }
}

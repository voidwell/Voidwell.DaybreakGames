using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusLoadout
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusLoadout(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusLoadoutModel>> GetAllLoadouts()
        {
            var query = _queryFactory.Create("loadout");

            query.ShowFields("loadout_id", "profile_id", "faction_id", "code_name");

            return await query.GetBatchAsync<CensusLoadoutModel>();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Services
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

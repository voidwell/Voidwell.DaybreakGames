using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Services
{
    public class CensusExperience
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusExperience(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusExperienceModel>> GetAllExperience()
        {
            var query = _queryFactory.Create("experience");

            query.ShowFields("experience_id", "description", "xp");

            return await query.GetBatchAsync<CensusExperienceModel>();
        }
    }
}

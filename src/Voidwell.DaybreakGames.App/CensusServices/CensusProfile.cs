using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusProfile
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusProfile(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusProfileModel>> GetAllProfiles()
        {
            var query = _queryFactory.Create("profile");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "profile_id",
                "profile_type_id",
                "faction_id",
                "name",
                "image_id"
            });

            return await query.GetBatchAsync<CensusProfileModel>();
        }
    }
}

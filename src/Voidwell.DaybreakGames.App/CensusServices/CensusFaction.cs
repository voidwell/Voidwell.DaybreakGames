using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusFaction
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusFaction(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusFactionModel>> GetAllFactions()
        {
            var query = _queryFactory.Create("faction");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "faction_id",
                "name",
                "image_id",
                "code_tag",
                "user_selectable"
            });

            return await query.GetBatchAsync<CensusFactionModel>();
        }
    }
}

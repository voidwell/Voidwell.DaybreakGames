using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusZone
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusZone(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusZoneModel>> GetAllZones()
        {
            var query = _queryFactory.Create("zone");
            query.SetLanguage("en");

            query.ShowFields("zone_id", "code", "name", "description", "hex_size");

            return await query.GetBatchAsync<CensusZoneModel>();
        }
    }
}

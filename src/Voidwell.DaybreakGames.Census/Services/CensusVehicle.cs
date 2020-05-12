using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Services
{
    public class CensusVehicle
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusVehicle(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusVehicleModel>> GetAllVehicles()
        {
            var query = _queryFactory.Create("vehicle");
            query.SetLanguage("en");

            query.ShowFields("vehicle_id", "name", "description", "cost", "cost_resource_id", "image_id");

            return await query.GetBatchAsync<CensusVehicleModel>();
        }

        public async Task<IEnumerable<CensusVehicleFactionModel>> GetAllVehicleFactions()
        {
            var query = _queryFactory.Create("vehicle_faction");
            query.SetLanguage("en");

            query.ShowFields("vehicle_id", "faction_id");

            return await query.GetBatchAsync<CensusVehicleFactionModel>();
        }
    }
}

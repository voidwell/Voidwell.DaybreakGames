using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusVehicle
    {
        private readonly ICensusClient _censusClient;

        public CensusVehicle(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusVehicleModel>> GetAllVehicles()
        {
            var query = _censusClient.CreateQuery("vehicle");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "vehicle_id",
                "name",
                "description",
                "cost",
                "cost_resource_id",
                "image_id"
            });

            return await query.GetBatch<CensusVehicleModel>();
        }

        public async Task<IEnumerable<CensusVehicleFactionModel>> GetAllVehicleFactions()
        {
            var query = _censusClient.CreateQuery("vehicle_faction");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "vehicle_id",
                "faction_id"
            });

            return await query.GetBatch<CensusVehicleFactionModel>();
        }
    }
}

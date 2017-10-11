using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusVehicle
    {
        public static async Task<IEnumerable<CensusVehicleModel>> GetAllVehicles()
        {
            var query = new CensusQuery.Query("vehicle");
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

        public static async Task<IEnumerable<CensusVehicleFactionModel>> GetAllVehicleFactions()
        {
            var query = new CensusQuery.Query("vehicle_faction");
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

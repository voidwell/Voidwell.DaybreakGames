using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class Vehicle
    {
        public static async Task<IEnumerable<JToken>> GetAllVehicles()
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

            return await query.GetBatch();
        }

        public static async Task<IEnumerable<JToken>> GetAllVehicleFactions()
        {
            var query = new CensusQuery.Query("vehicle_faction");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "vehicle_id",
                "faction_id"
            });

            return await query.GetBatch();
        }
    }
}

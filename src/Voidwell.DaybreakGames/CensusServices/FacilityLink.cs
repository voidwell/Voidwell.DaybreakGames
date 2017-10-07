using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class FacilityLink
    {
        public static async Task<IEnumerable<JToken>> GetAllFacilityLinks()
        {
            var query = new CensusQuery.Query("facility_link");

            query.ShowFields(new[]
            {
                "zone_id",
                "facility_id_a",
                "facility_id_b",
                "description"
            });

            return await query.GetBatch();
        }
    }
}

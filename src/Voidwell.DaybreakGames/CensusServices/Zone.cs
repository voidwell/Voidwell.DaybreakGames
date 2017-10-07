using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class Zone
    {
        public static async Task<IEnumerable<JToken>> GetAllZones()
        {
            var query = new CensusQuery.Query("zone");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "zone_id",
                "code",
                "name",
                "description",
                "hex_size"
            });

            return await query.GetBatch();
        }
    }
}

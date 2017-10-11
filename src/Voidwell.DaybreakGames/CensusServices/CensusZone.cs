using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusZone
    {
        public static async Task<IEnumerable<CensusZoneModel>> GetAllZones()
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

            return await query.GetBatch<CensusZoneModel>();
        }
    }
}

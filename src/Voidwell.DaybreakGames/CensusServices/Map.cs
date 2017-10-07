using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class Map
    {
        public static async Task<IEnumerable<JToken>> GetMapOwnership(string worldId, string zoneId)
        {
            var query = new CensusQuery.Query("map");
            query.SetLanguage("en");

            query.Where("world_id").Equals(worldId);
            query.Where("zone_ids").Equals(zoneId);

            return await query.GetBatch();
        }

        public static async Task<IEnumerable<JToken>> GetAllMapHexs()
        {
            var query = new CensusQuery.Query("map_hex");

            query.ShowFields(new[]
            {
                "zone_id",
                "map_region_id",
                "x",
                "y",
                "hex_type",
                "type_name"
            });

            return await query.GetBatch();
        }

        public static async Task<IEnumerable<JToken>> GetAllMapRegions()
        {
            var query = new CensusQuery.Query("map_region");

            query.ShowFields(new[]
            {
                "map_region_id",
                "zone_id",
                "facility_id",
                "facility_name",
                "facility_type_id",
                "facility_type",
                "location_x",
                "location_y",
                "location_z"
            });

            return await query.GetBatch();
        }
    }
}

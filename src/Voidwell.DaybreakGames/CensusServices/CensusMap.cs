using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusMap
    {
        private readonly ICensusClient _censusClient;

        public CensusMap(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<CensusMapModel> GetMapOwnership(string worldId, string zoneId)
        {
            var query = _censusClient.CreateQuery("map");
            query.SetLanguage("en");

            query.Where("world_id").Equals(worldId);
            query.Where("zone_ids").Equals(zoneId);

            return await query.Get<CensusMapModel>();
        }

        public async Task<IEnumerable<CensusMapHexModel>> GetAllMapHexs()
        {
            var query = _censusClient.CreateQuery("map_hex");

            query.ShowFields(new[]
            {
                "zone_id",
                "map_region_id",
                "x",
                "y",
                "hex_type",
                "type_name"
            });

            return await query.GetBatch<CensusMapHexModel>();
        }

        public async Task<IEnumerable<CensusMapRegionModel>> GetAllMapRegions()
        {
            var query = _censusClient.CreateQuery("map_region");

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

            return await query.GetBatch<CensusMapRegionModel>();
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetAllFacilityLinks()
        {
            var query = _censusClient.CreateQuery("facility_link");

            query.ShowFields(new[]
            {
                "zone_id",
                "facility_id_a",
                "facility_id_b",
                "description"
            });

            return await query.GetBatch<CensusFacilityLinkModel>();
        }
    }
}

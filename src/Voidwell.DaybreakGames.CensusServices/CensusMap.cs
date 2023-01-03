using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusMap : ICensusMap
    {
        private readonly ICensusClient _client;

        public CensusMap(ICensusClient client)
        {
            _client = client;
        }

        public async Task<CensusMapModel> GetMapOwnership(int worldId, int zoneId)
        {
            var query = _client.CreateQuery("map");
            query.SetLanguage("en");

            query.Where("world_id").Equals(worldId);
            query.Where("zone_ids").Equals(zoneId);

            try
            {
                return await query.GetAsync<CensusMapModel>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CensusMapHexModel>> GetAllMapHexs()
        {
            var query = _client.CreateQuery("map_hex");

            query.ShowFields("zone_id", "map_region_id", "x", "y", "hex_type", "type_name");

            return await query.GetBatchAsync<CensusMapHexModel>();
        }

        public async Task<IEnumerable<CensusMapRegionModel>> GetAllMapRegions()
        {
            var query = _client.CreateQuery("map_region");

            query.ShowFields("map_region_id", "zone_id", "facility_id", "facility_name", "facility_type_id", "facility_type", "location_x", "location_y", "location_z");

            return await query.GetBatchAsync<CensusMapRegionModel>();
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetAllFacilityLinks()
        {
            var query = _client.CreateQuery("facility_link");

            query.ShowFields("zone_id", "facility_id_a", "facility_id_b", "description");

            return await query.GetBatchAsync<CensusFacilityLinkModel>();
        }
    }
}

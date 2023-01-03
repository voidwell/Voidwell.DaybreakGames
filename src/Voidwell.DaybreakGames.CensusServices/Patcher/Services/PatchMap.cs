using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices.Patcher.Services
{
    public class PatchMap : ICensusMap
    {
        private readonly IPatchClient _patchClient;
        private readonly CensusMap _censusClient;

        public PatchMap(IPatchClient client, CensusMap censusMap)
        {
            _patchClient = client;
            _censusClient = censusMap;
        }

        public Task<CensusMapModel> GetMapOwnership(int worldId, int zoneId)
        {
            return _censusClient.GetMapOwnership(worldId, zoneId);
        }

        public async Task<IEnumerable<CensusMapHexModel>> GetAllMapHexs()
        {
            var censusResults = await _censusClient.GetAllMapHexs();

            var query = _patchClient.CreateQuery("map_hex");

            query.ShowFields("zone_id", "map_region_id", "x", "y", "hex_type", "type_name");

            var patchResults = await query.GetBatchAsync<CensusMapHexModel>();

            return PatchUtil.PatchData<CensusMapHexModel>(x => $"{x.ZoneId}-{x.X}-{x.Y}", censusResults, patchResults);
        }

        public async Task<IEnumerable<CensusMapRegionModel>> GetAllMapRegions()
        {
            var censusResults = await _censusClient.GetAllMapRegions();

            var query = _patchClient.CreateQuery("map_region");

            query.ShowFields("map_region_id", "zone_id", "facility_id", "facility_name", "facility_type_id", "facility_type", "location_x", "location_y", "location_z");

            var patchResults = await query.GetBatchAsync<CensusMapRegionModel>();

            return PatchUtil.PatchData<CensusMapRegionModel>(x => x.MapRegionId, censusResults, patchResults);
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetAllFacilityLinks()
        {
            var censusResults = await _censusClient.GetAllFacilityLinks();

            var query = _patchClient.CreateQuery("facility_link");

            query.ShowFields("zone_id", "facility_id_a", "facility_id_b", "description");

            var patchResults = await query.GetBatchAsync<CensusFacilityLinkModel>();

            return PatchUtil.PatchData<CensusFacilityLinkModel>(x => $"{x.ZoneId}-{x.FacilityIdA}-{x.FacilityIdB}", censusResults, patchResults);
        }
    }
}

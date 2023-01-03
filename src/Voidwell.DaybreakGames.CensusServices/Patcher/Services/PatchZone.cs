using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices.Patcher.Services
{
    public class PatchZone : ICensusZone
    {
        private readonly IPatchClient _patchClient;
        private readonly CensusZone _censusClient;

        public PatchZone(IPatchClient client, CensusZone censusZone)
        {
            _patchClient = client;
            _censusClient = censusZone;
        }

        public async Task<IEnumerable<CensusZoneModel>> GetAllZones()
        {
            var censusResults = await _censusClient.GetAllZones();

            var query = _patchClient.CreateQuery("zone");
            query.SetLanguage("en");

            query.ShowFields("zone_id", "code", "name", "description", "hex_size");

            var patchResults = await query.GetBatchAsync<CensusZoneModel>();

            return PatchUtil.PatchData<CensusZoneModel>(x => x.ZoneId, censusResults, patchResults);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusZone : ICensusZone
    {
        private readonly ICensusClient _client;

        public CensusZone(ICensusClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CensusZoneModel>> GetAllZones()
        {
            var query = _client.CreateQuery("zone");
            query.SetLanguage("en");

            query.ShowFields("zone_id", "code", "name", "description", "hex_size");

            return await query.GetBatchAsync<CensusZoneModel>();
        }
    }
}

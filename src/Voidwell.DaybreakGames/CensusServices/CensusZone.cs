using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusZone
    {
        private readonly ICensusClient _censusClient;

        public CensusZone(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusZoneModel>> GetAllZones()
        {
            var query = _censusClient.CreateQuery("zone");
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

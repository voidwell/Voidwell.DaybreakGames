using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusWorldEvent
    {
        private readonly ICensusClient _censusClient;

        public CensusWorldEvent(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusMetagameWorldEventModel>> GetMetagameWorldEvents()
        {
            var query = _censusClient.CreateQuery("world_event");
            query.Where("type").Equals("METAGAME");

            return await query.GetList<CensusMetagameWorldEventModel>();
        }
    }
}

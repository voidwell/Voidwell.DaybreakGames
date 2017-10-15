using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusWorld
    {
        private readonly ICensusClient _censusClient;

        public CensusWorld(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusWorldModel>> GetAllWorlds ()
        {
            var query = _censusClient.CreateQuery("world");
            query.SetLanguage("en");

            return await query.GetBatch<CensusWorldModel>();
        }
    }
}

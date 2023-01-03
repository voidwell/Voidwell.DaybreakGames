using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusWorld : ICensusWorld
    {
        private readonly ICensusClient _client;

        public CensusWorld(ICensusClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CensusWorldModel>> GetAllWorlds()
        {
            var query = _client.CreateQuery("world");
            query.SetLanguage("en");

            return await query.GetBatchAsync<CensusWorldModel>();
        }
    }
}

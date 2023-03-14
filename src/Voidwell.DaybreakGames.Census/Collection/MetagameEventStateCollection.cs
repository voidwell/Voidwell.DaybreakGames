using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MetagameEventStateCollection : ICensusStaticCollection<CensusMetagameEventStateModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "metagame_event_state";

        public MetagameEventStateCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusMetagameEventStateModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("metagame_event_state_id", "name")
                .GetBatchAsync<CensusMetagameEventStateModel>();
        }
    }
}

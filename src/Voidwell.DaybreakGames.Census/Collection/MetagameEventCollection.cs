using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MetagameEventCollection : ICensusStaticCollection<CensusMetagameEventCategoryModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "metagame_event";

        public MetagameEventCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusMetagameEventCategoryModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("metagame_event_id", "name", "description", "type", "experience_bonus")
                .GetBatchAsync<CensusMetagameEventCategoryModel>();
        }
    }
}

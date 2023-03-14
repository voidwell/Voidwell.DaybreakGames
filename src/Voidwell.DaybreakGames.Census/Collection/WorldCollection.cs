using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class WorldCollection : ICensusStaticCollection<CensusWorldModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "world";

        public WorldCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusWorldModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .GetBatchAsync<CensusWorldModel>();
        }
    }
}

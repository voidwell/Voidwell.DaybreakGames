using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ObjectiveCollection : ICensusStaticCollection<CensusObjectiveModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "objective";

        public ObjectiveCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusObjectiveModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .GetBatchAsync<CensusObjectiveModel>();
        }
    }
}

using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ObjectiveSetToObjectiveCollection : ICensusStaticCollection<CensusObjectiveSetToObjectiveModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "objective_set_to_objective";

        public ObjectiveSetToObjectiveCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusObjectiveSetToObjectiveModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .GetBatchAsync<CensusObjectiveSetToObjectiveModel>();
        }
    }
}

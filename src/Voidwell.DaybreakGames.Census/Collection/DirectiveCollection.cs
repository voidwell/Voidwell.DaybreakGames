using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveCollection : ICensusStaticCollection<CensusDirectiveModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "directive";

        public DirectiveCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusDirectiveModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .GetBatchAsync<CensusDirectiveModel>();
        }
    }
}

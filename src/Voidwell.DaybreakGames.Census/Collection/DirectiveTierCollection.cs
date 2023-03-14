using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveTierCollection : ICensusStaticCollection<CensusDirectiveTierModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "directive_tier";

        public DirectiveTierCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusDirectiveTierModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .GetBatchAsync<CensusDirectiveTierModel>();
        }
    }
}

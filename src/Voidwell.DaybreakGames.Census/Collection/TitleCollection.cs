using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class TitleCollection : ICensusStaticCollection<CensusTitleModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "title";

        public TitleCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusTitleModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("title_id", "name")
                .GetBatchAsync<CensusTitleModel>();
        }
    }
}

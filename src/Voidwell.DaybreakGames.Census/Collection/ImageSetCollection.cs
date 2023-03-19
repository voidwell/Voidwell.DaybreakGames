using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ImageSetCollection : ICensusStaticCollection<CensusImageSetModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "image_set";

        public ImageSetCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusImageSetModel>> GetCollectionAsync()
        {
            return await _client
                .CreateQuery(CollectionName)
                .GetBatchAsync<CensusImageSetModel>();
        }
    }
}

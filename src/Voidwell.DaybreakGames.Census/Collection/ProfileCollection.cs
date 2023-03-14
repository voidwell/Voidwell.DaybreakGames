using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ProfileCollection : ICensusStaticCollection<CensusProfileModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "profile";

        public ProfileCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusProfileModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("profile_id", "profile_type_id", "faction_id", "name", "image_id")
                .GetBatchAsync<CensusProfileModel>();
        }
    }
}

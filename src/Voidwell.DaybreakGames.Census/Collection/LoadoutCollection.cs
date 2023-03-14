using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class LoadoutCollection : ICensusStaticCollection<CensusLoadoutModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "loadout";

        public LoadoutCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusLoadoutModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .ShowFields("loadout_id", "profile_id", "faction_id", "code_name")
                .GetBatchAsync<CensusLoadoutModel>();
        }
    }
}

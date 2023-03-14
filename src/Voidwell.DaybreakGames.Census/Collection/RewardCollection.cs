using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class RewardCollection : ICensusStaticCollection<CensusRewardModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "reward";

        public RewardCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusRewardModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .GetBatchAsync<CensusRewardModel>();
        }
    }
}

using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class RewardGroupToRewardCollection : ICensusStaticCollection<CensusRewardGroupToRewardModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "reward_group_to_reward";

        public RewardGroupToRewardCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusRewardGroupToRewardModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .GetBatchAsync<CensusRewardGroupToRewardModel>();
        }
    }
}

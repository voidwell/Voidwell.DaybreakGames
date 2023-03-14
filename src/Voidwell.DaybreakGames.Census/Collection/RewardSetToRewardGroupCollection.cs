using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class RewardSetToRewardGroupCollection : ICensusStaticCollection<CensusRewardSetToRewardGroupModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "reward_set_to_reward_group";

        public RewardSetToRewardGroupCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusRewardSetToRewardGroupModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .GetBatchAsync<CensusRewardSetToRewardGroupModel>();
        }
    }
}

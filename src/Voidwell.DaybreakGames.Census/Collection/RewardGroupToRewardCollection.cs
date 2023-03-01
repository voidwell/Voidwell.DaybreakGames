using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class RewardGroupToRewardCollection : CensusCollection, ICensusStaticCollection<CensusRewardGroupToRewardModel>
    {
        public override string CollectionName => "reward_group_to_reward";

        public RewardGroupToRewardCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusRewardGroupToRewardModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                return query.GetBatchAsync<CensusRewardGroupToRewardModel>();
            });
        }
    }
}

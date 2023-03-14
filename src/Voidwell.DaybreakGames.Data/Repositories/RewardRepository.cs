using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public RewardRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<RewardSetToRewardGroup>> GetRewardSetsAsync(IEnumerable<int> rewardSetIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var rewardSets = await dbContext.RewardSetsToRewardGroup
                    .Where(a => rewardSetIds.Contains(a.RewardSetId))
                    .ToListAsync();

                var rewardGroupIds = rewardSets.Select(a => a.RewardGroupId).Distinct();
                var rewardGroups = await (from rewardGroup in dbContext.RewardGroupsToReward
                                          where rewardGroupIds.Contains(rewardGroup.RewardGroupId)

                                          join reward in dbContext.Rewards on rewardGroup.RewardId equals reward.Id into rewards
                                          from reward in rewards.DefaultIfEmpty()

                                          join item in dbContext.Items on reward.Param1 equals item.Id into items
                                          from item in items.DefaultIfEmpty()

                                          select new RewardGroupToReward
                                          {
                                              RewardGroupId = rewardGroup.RewardGroupId,
                                              RewardId = rewardGroup.RewardId,
                                              Reward = reward == null ? null : new Reward
                                              {
                                                  Id = reward.Id,
                                                  RewardTypeId = reward.RewardTypeId,
                                                  CountMax = reward.CountMax,
                                                  CountMin = reward.CountMin,
                                                  Param1 = reward.Param1,
                                                  Param2 = reward.Param2,
                                                  Param3 = reward.Param3,
                                                  Param4 = reward.Param4,
                                                  Param5 = reward.Param5,
                                                  Item = item
                                              }
                                          }).ToListAsync();

                rewardSets.SetGroupJoin(rewardGroups, a => a.RewardGroupId, a => a.RewardGroupId, a => a.RewardGroups);

                return rewardSets;
            }
        }
    }
}

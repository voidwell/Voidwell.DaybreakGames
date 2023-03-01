using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class DirectiveRepository : IDirectiveRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public DirectiveRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Directive>> GetDirectivesAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var test = await dbContext.DirectiveTiers.Select(a => new { a.RewardSetId }).ToListAsync();

                var query = dbContext.Directives
                                .Include(a => a.ObjectiveSet)
                                    .ThenInclude(a => a.Objectives);

                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<DirectiveTreeCategory>> GetDirectiveTreesCategoriesAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var treeCategories = await dbContext.DirectiveTreeCategories.ToListAsync();
                var trees = await dbContext.DirectiveTrees.ToListAsync();
                var treeTiers = await dbContext.DirectiveTiers.ToListAsync();

                var rewardSetIds = treeTiers.Select(a => a.RewardSetId).Distinct();
                var rewardSets = await dbContext.RewardSetsToRewardGroup.Where(a => rewardSetIds.Contains(a.RewardSetId)).ToListAsync();

                var rewardGroupIds = rewardSets.Select(a => a.RewardSetId).Distinct();
                var rewardGroups = await (from rewardGroup in dbContext.RewardGroupsToReward
                                          where rewardGroupIds.Contains(rewardGroup.RewardGroupId)
                                          join reward in dbContext.Rewards on rewardGroup.RewardId equals reward.Id
                                          join item in dbContext.Items on reward.Param1 equals item.Id
                                          select new RewardGroupToReward
                                          {
                                              RewardGroupId = rewardGroup.RewardGroupId,
                                              RewardId = rewardGroup.RewardId,
                                              Reward = new Reward
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

                var directives = await (from directive in dbContext.Directives
                                        join objectiveSet in dbContext.ObjectiveSetsToObjective on directive.ObjectiveSetId equals objectiveSet.ObjectiveSetId
                                        select new Directive
                                        {
                                            Id = directive.Id,
                                            Name = directive.Name,
                                            Description = directive.Description,
                                            DirectiveTierId = directive.DirectiveTierId,
                                            DirectiveTreeId = directive.DirectiveTreeId,
                                            ImageId = directive.ImageId,
                                            ImageSetId = directive.ImageSetId,
                                            ObjectiveSetId = directive.ObjectiveSetId,
                                            QualifyRequirementId = directive.QualifyRequirementId,
                                            ObjectiveSet = new ObjectiveSetToObjective
                                            {
                                                ObjectiveSetId = objectiveSet.ObjectiveSetId,
                                                ObjectiveGroupId = objectiveSet.ObjectiveGroupId
                                            }
                                        }).ToListAsync();

                var objectiveGroupIds = directives.Where(a => a.ObjectiveSet != null).Select(a => a.ObjectiveSet.ObjectiveGroupId).Distinct();

                var achievementObjectives = await (from objective in dbContext.Objectives
                                                   where objective.ObjectiveTypeId == 66 && objectiveGroupIds.Contains(objective.ObjectiveGroupId)
                                                   join achievement in dbContext.Achievements on objective.Param1 equals achievement.Id.ToString()
                                                   join achObjective in dbContext.Objectives on achievement.ObjectiveGroupId equals achObjective.ObjectiveGroupId
                                                   join item in dbContext.Items on achObjective.Param5 equals item.Id.ToString()
                                                   select new Objective
                                                   {
                                                       Id = objective.Id,
                                                       ObjectiveGroupId = objective.ObjectiveGroupId,
                                                       ObjectiveTypeId = objective.ObjectiveTypeId,
                                                       Param1 = objective.Param1,
                                                       Param2 = objective.Param2,
                                                       Param3 = objective.Param3,
                                                       Param4 = objective.Param4,
                                                       Param5 = objective.Param5,
                                                       Param6 = objective.Param6,
                                                       Param7 = objective.Param7,
                                                       Param8 = objective.Param8,
                                                       Param9 = objective.Param9,
                                                       Achievement = new Achievement
                                                       {
                                                           Id = achievement.Id,
                                                           Name = achievement.Name,
                                                           Description = achievement.Description,
                                                           ImageId = achievement.ImageId,
                                                           ObjectiveGroupId = achievement.ObjectiveGroupId,
                                                           ItemId = achievement.ItemId,
                                                           Repeatable = achievement.Repeatable,
                                                           RewardId = achievement.RewardId,
                                                           Objective = new Objective
                                                           {
                                                               Id = achObjective.Id,
                                                               ObjectiveGroupId = achObjective.ObjectiveGroupId,
                                                               ObjectiveTypeId = achObjective.ObjectiveTypeId,
                                                               Param1 = achObjective.Param1,
                                                               Param2 = achObjective.Param2,
                                                               Param3 = achObjective.Param3,
                                                               Param4 = achObjective.Param4,
                                                               Param5 = achObjective.Param5,
                                                               Param6 = achObjective.Param6,
                                                               Param7 = achObjective.Param7,
                                                               Param8 = achObjective.Param8,
                                                               Param9 = achObjective.Param9,
                                                               Item = item
                                                           }
                                                       }
                                                   }).ToListAsync();
                var nonAchievementObjectives = await dbContext.Objectives.Where(a => a.ObjectiveTypeId != 66 && objectiveGroupIds.Contains(a.ObjectiveGroupId)).ToListAsync();
                var objectives = achievementObjectives.Union(nonAchievementObjectives);

                directives.SetGroup(objectives, a => a.ObjectiveSet.ObjectiveGroupId, a => a.ObjectiveGroupId, (a, b) => a.ObjectiveSet.Objectives = b);
                rewardSets.SetGroup(rewardGroups, a => a.RewardGroupId, a => a.RewardGroupId, (a, b) => a.RewardGroups = b);
                treeTiers.SetGroup(rewardSets, a => a.RewardSetId, a => a.RewardSetId, (a, b) => a.RewardGroupSets = b);
                treeTiers.SetGroup(directives, a => new { a.DirectiveTreeId, a.DirectiveTierId }, a => new { a.DirectiveTreeId, a.DirectiveTierId }, (a, b) => a.Directives = b);
                trees.SetGroup(treeTiers, a => a.Id, a => a.DirectiveTreeId, (a, b) => a.Tiers = b);
                treeCategories.SetGroup(trees, a => a.Id, a => a.DirectiveTreeCategoryId, (a, b) => a.Trees = b);

                return treeCategories;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class ObjectiveRepository : IObjectiveRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        private const int TotalKillsObjectiveTypeId = 12;
        private const int AchievementObjectiveTypeId = 66;

        public ObjectiveRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Objective>> GetObjectivesByGroupIdAsync(IEnumerable<int> objectiveGroupIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from objective in dbContext.Objectives
                            where objectiveGroupIds.Contains(objective.ObjectiveGroupId)

                            join item in dbContext.Items
                                on new { typeId = objective.ObjectiveTypeId, itemId = objective.Param5 }
                                equals new { typeId = TotalKillsObjectiveTypeId, itemId = item.Id.ToString() }
                                into items
                            from item in items.DefaultIfEmpty()

                            join achievement in dbContext.Achievements
                                on new { typeId = objective.ObjectiveTypeId, achievementId = objective.Param1 }
                                equals new { typeId = AchievementObjectiveTypeId, achievementId = achievement.Id.ToString() }
                                into achievements
                            from achievement in achievements.DefaultIfEmpty()

                            join achObjective in dbContext.Objectives
                                on achievement.ObjectiveGroupId
                                equals achObjective.ObjectiveGroupId
                                into achObjectives
                            from achObjective in achObjectives.DefaultIfEmpty()

                            join achItem in dbContext.Items
                                on new { typeId = achObjective.ObjectiveTypeId, itemId = achObjective.Param5 } 
                                equals new { typeId = TotalKillsObjectiveTypeId, itemId = achItem.Id.ToString() }
                                into achItems
                            from achItem in achItems.DefaultIfEmpty()

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
                                Item = item,
                                Achievement = achievement == null ? null : new Achievement
                                {
                                    Id = achievement.Id,
                                    Name = achievement.Name,
                                    Description = achievement.Description,
                                    ImageId = achievement.ImageId,
                                    ObjectiveGroupId = achievement.ObjectiveGroupId,
                                    ItemId = achievement.ItemId,
                                    Repeatable = achievement.Repeatable,
                                    RewardId = achievement.RewardId,
                                    Objective = achObjective == null ? null : new Objective
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
                                        Item = achItem
                                    }
                                }
                            };
                return await query.ToListAsync();
            }
        }
    }
}

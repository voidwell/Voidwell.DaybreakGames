using System.Threading.Tasks;
using System;
using Voidwell.Microservice.Cache;
using System.Linq;
using Voidwell.DaybreakGames.Data.Repositories;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class DirectiveService : IDirectiveService
    {
        private readonly IDirectiveRepository _directiveRepository;
        private readonly IRewardRepository _rewardRepository;
        private readonly IObjectiveRepository _objectiveRepository;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.directive";
        private readonly Func<string> _getDirectiveDataCacheKey = () => $"{_cacheKey}_directive_data";

        private readonly TimeSpan _cacheDirectiveDataExpiration = TimeSpan.FromMinutes(60);

        public DirectiveService(IDirectiveRepository directiveRepository, IRewardRepository rewardRepository,
            IObjectiveRepository objectiveRepository, ICache cache)
        {
            _directiveRepository = directiveRepository;
            _rewardRepository = rewardRepository;
            _objectiveRepository = objectiveRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<DirectiveTreeCategory>> GetDirectiveDataAsync()
        {
            var cacheKey = _getDirectiveDataCacheKey();

            var data = await _cache.GetAsync<IEnumerable<DirectiveTreeCategory>>(cacheKey);
            if (data != null)
            {
                //return data;
            }

            var categoriesTask = _directiveRepository.GetDirectiveTreesCategoriesAsync();
            var treesTask = _directiveRepository.GetDirectiveTreesAsync();
            var tiersTask = _directiveRepository.GetDirectiveTiersAsync();
            var directivesTask = _directiveRepository.GetDirectivesAsync();

            await Task.WhenAll(categoriesTask, treesTask, tiersTask, directivesTask);

            var categories = categoriesTask.Result.ToList();
            var trees = treesTask.Result.ToList();
            var tiers = tiersTask.Result.ToList();
            var directives = directivesTask.Result.ToList();

            var objectiveGroupIds = directives.Where(a => a.ObjectiveSet?.ObjectiveGroupId != null).Select(a => a.ObjectiveSet.ObjectiveGroupId).Distinct();
            var objectivesTask = _objectiveRepository.GetObjectivesByGroupIdAsync(objectiveGroupIds);

            var rewardSetIds = tiers.Where(a => a.RewardSetId != null).Select(a => a.RewardSetId.Value).Distinct();
            var rewardSetTask = _rewardRepository.GetRewardSetsAsync(rewardSetIds);

            await Task.WhenAll(objectivesTask, rewardSetTask);

            var objectives = objectivesTask.Result;
            var rewardSets = rewardSetTask.Result;

            directives
                .Where(a => a.ObjectiveSet != null)
                .Select(a => a.ObjectiveSet)
                .SetGroupJoin(objectives, a => a.ObjectiveGroupId, a => a.ObjectiveGroupId, a => a.Objectives);

            tiers.SetGroupJoin(directives, a => new { a.DirectiveTreeId, a.DirectiveTierId }, a => new { a.DirectiveTreeId, a.DirectiveTierId }, a => a.Directives);
            tiers.SetGroupJoin(rewardSets, a => a.RewardSetId, a => a.RewardSetId, a => a.RewardGroupSets);
            trees.SetGroupJoin(tiers, a => a.Id, a => a.DirectiveTreeId, a => a.Tiers);
            categories.SetGroupJoin(trees, a => a.Id, a => a.DirectiveTreeCategoryId, a => a.Trees);

            if (categories != null)
            {
                await _cache.SetAsync(cacheKey, categories, _cacheDirectiveDataExpiration);
            }

            return categories;
        }
    }
}

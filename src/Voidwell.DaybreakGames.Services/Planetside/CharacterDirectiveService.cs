using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Utils;
using Voidwell.Microservice.Cache;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterDirectiveService : ICharacterDirectiveService
    {
        private readonly ICharacterDirectiveStore _characterDirectiveStore;
        private readonly ICharacterService _characterService;
        private readonly IDirectiveService _directiveService;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        private const string _cacheKey = "ps2.characterDirective";
        private readonly Func<string, string> _getDirectivesCacheKey = (characterId) => $"{_cacheKey}_character_{characterId}";

        private readonly TimeSpan _cacheDirectivesCharacterExpiration = TimeSpan.FromMinutes(10);

        public CharacterDirectiveService(ICharacterDirectiveStore characterDirectiveStore, ICharacterService characterService,
            IDirectiveService directiveService, ICache cache, IMapper mapper)
        {
            _characterDirectiveStore = characterDirectiveStore;
            _characterService = characterService;
            _directiveService = directiveService;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<CharacterDirectivesOutline> GetCharacterDirectivesAsync(string characterId)
        {
            var cacheKey = _getDirectivesCacheKey(characterId);

            var data = await _cache.GetAsync<CharacterDirectivesOutline>(cacheKey);
            if (data != null)
            {
                return data;
            }

            var character = await _characterService.GetCharacter(characterId);

            var characterTreesTask = _characterDirectiveStore.GetCharacterDirectivesAsync(characterId);
            var categoriesTask = _directiveService.GetDirectiveDataAsync();
            var characterAchievementsTask = _characterService.GetCharacterAchievementsAsync(characterId);
            var characterWeaponsTask = _characterService.GetWeaponStatsAsync(characterId);

            await Task.WhenAll(characterTreesTask, categoriesTask, characterAchievementsTask, characterWeaponsTask);

            var characterTrees = characterTreesTask.Result;
            var categories = categoriesTask.Result;
            var characterAchievements = characterAchievementsTask.Result;
            var characterWeapons = characterWeaponsTask.Result;

            if (character == null || characterTrees == null || !characterTrees.Any())
            {
                return null;
            }

            foreach (var category in categories)
            {
                category.Trees = category.Trees.Where(a => characterTrees.Any(b => b.DirectiveTreeId == a.Id));
            }
            categories = categories.Where(a => a.Trees.Any());

            data = new CharacterDirectivesOutline();

            foreach (var category in categories)
            {
                var outlineCategory = _mapper.Map<CharacterDirectivesOutlineCategory>(category);

                foreach (var tree in category.Trees.OrderBy(a => a.Name))
                {
                    var characterTree = characterTrees.FirstOrDefault(a => a.DirectiveTreeId == tree.Id);
                    if (characterTree != null)
                    {
                        var outlineTree = _mapper.Map<CharacterDirectivesOutlineTree>(tree);
                        outlineCategory.Trees.Add(outlineTree);

                        _mapper.Map(characterTree, outlineTree);

                        foreach (var tier in tree.Tiers)
                        {
                            var outlineTier = _mapper.Map<CharacterDirectivesOutlineTier>(tier);
                            outlineTree.Tiers.Add(outlineTier);

                            outlineTier.Rewards = GetTierRewardsForFactionId(tier, character.FactionId);

                            var characterTier = characterTree?.CharacterDirectiveTiers.FirstOrDefault(a => a.DirectiveTierId == tier.DirectiveTierId && a.DirectiveTreeId == tier.DirectiveTreeId);
                            if (characterTier != null)
                            {
                                _mapper.Map(characterTier, outlineTier);
                            }

                            foreach (var directive in tier.Directives)
                            {
                                var storeObjective = directive.ObjectiveSet.Objectives.FirstOrDefault();

                                var objectiveFactionId = GetFactionIdForDirective(directive);
                                if (objectiveFactionId != null && objectiveFactionId != 0 && objectiveFactionId != character.FactionId)
                                {
                                    continue;
                                }

                                if (tier.Directives.Any(d => d.Id != directive.Id && d.Name == directive.Name && GetFactionIdForDirective(d) == character.FactionId))
                                {
                                    continue;
                                }

                                var outlineDirective = _mapper.Map<CharacterDirectivesOutlineDirective>(directive);
                                outlineTier.Directives.Add(outlineDirective);

                                outlineDirective.Objective.GoalValue = GetObjectiveTargetValue(storeObjective);

                                var characterDirective = characterTier?.CharacterDirectives.FirstOrDefault(a => a.DirectiveId == directive.Id);
                                if (characterDirective != null)
                                {
                                    _mapper.Map(characterDirective, outlineDirective);
                                }

                                if (outlineTier.CompletionDate == null)
                                {
                                    if (storeObjective != null && storeObjective.ObjectiveTypeId == 66)
                                    {
                                        var characterAchievement = characterAchievements.FirstOrDefault(a => a.AchievementId.ToString() == storeObjective.Param1);
                                        outlineDirective.Progress = GetAchievementProgress(characterAchievement, characterWeapons) ?? 0;
                                    }
                                    else
                                    {
                                        outlineDirective.Progress = characterDirective?.CharacterDirectiveObjectives?.FirstOrDefault()?.StateData ?? 0;
                                    }
                                };
                            }

                            outlineTier.Directives = outlineTier.Directives.Where(a => a.CompletionDate != null).OrderBy(a => a.CompletionDate).ThenBy(a => a.Id)
                                .Union(outlineTier.Directives.Where(a => a.CompletionDate == null).OrderByDescending(a => (double)a.Progress / a.Objective.GoalValue.GetValueOrDefault()).ThenBy(a => a.Id))
                                .ToList();

                            outlineTier.CompletionPercent = GetTierCompletionPercent(outlineTier);
                        }
                    }
                }

                if (outlineCategory.Trees.Any())
                {
                    data.Categories.Add(outlineCategory);
                }
            }

            if (data != null)
            {
                await _cache.SetAsync(cacheKey, data, _cacheDirectivesCharacterExpiration);
            }

            return data;
        }

        public Task UpdateCharacterDirectivesAsync(string characterId)
        {
            return _characterDirectiveStore.UpdateCharacterDirectiveDataAsync(characterId);
        }

        private List<DirectivesOutlineReward> GetTierRewardsForFactionId(DirectiveTier tier, int factionId)
        {
            var factionRewards = tier.RewardGroupSets.SelectManyNotNull(a => a.RewardGroups)
                                .Where(a => a.Reward?.Item?.FactionId == null || a.Reward?.Item?.FactionId == 0 || a.Reward?.Item?.FactionId == factionId);
            return _mapper.Map<List<DirectivesOutlineReward>>(factionRewards);
        }

        private static double GetTierCompletionPercent(CharacterDirectivesOutlineTier tier)
        {
            if (tier.CompletionDate != null)
            {
                return 100;
            }

            if (tier.Directives.Any())
            {
                var directiveAverage = tier.Directives
                    .Select(a =>
                    {
                        if (a.CompletionDate != null)
                        {
                            return 1.0;
                        }

                        if (a.Objective?.GoalValue != null)
                        {
                            return (double)a.Progress / a.Objective.GoalValue.GetValueOrDefault();
                        }

                        return 0;
                    })
                    .OrderByDescending(a => a)
                    .Take(tier.CompletionCount)
                    .Average();

                if (!double.IsNaN(directiveAverage) && !double.IsInfinity(directiveAverage))
                {
                    return Math.Round(directiveAverage * 100, 2);
                }
            }

            return 0;
        }

        private static int? GetFactionIdForDirective(Directive directive)
        {
            var storeObjective = directive.ObjectiveSet.Objectives.FirstOrDefault();
            return storeObjective.Item?.FactionId ?? storeObjective.Achievement?.Objective?.Item?.FactionId;
        }

        private int? GetObjectiveTargetValue(Objective storeObjective)
        {
            return GetGoalValueForObjective(storeObjective);
        }

        private static int? GetGoalValueForObjective(Objective objective)
        {
            string param = null;

            if (objective?.Achievement?.Objective != null)
            {
                if (objective.Param3 == null)
                {
                    return GetGoalValueForObjective(objective?.Achievement?.Objective);
                }

                param = objective.Param3;
            }

            if (param == null)
            {
                switch (objective.ObjectiveTypeId)
                {
                    case 3:
                    case 12:
                    case 14:
                    case 15:
                    case 17:
                    case 20:
                    case 35:
                    case 69:
                    case 90:
                    case 91:
                    case 92:
                    case 93: param = objective.Param1; break;
                    case 19: param = objective.Param2; break;
                    case 89: param = objective.Param5; break;
                    default: param = objective.Param1; break;
                }
            }

            if (param != null && int.TryParse(param, out var paramVal))
            {
                return paramVal;
            }

            return null;
        }

        private int? GetAchievementProgress(CharacterAchievement characterAchievement, IEnumerable<CharacterWeaponStat> weaponStats)
        {
            if (characterAchievement == null)
            {
                return null;
            }

            var itemId = characterAchievement?.Achievement?.ItemId?.ToString();

            var achObjective = characterAchievement?.Achievement?.Objective;
            if (achObjective != null && achObjective.ObjectiveTypeId == 12)
            {
                if (achObjective.Param5 != null)
                {
                    itemId = achObjective?.Param5;
                }

                return null;
            }

            if (itemId != null && itemId != "0")
            {
                return weaponStats
                        .OrderByDescending(a => a.Kills)
                        .FirstOrDefault(a => a.ItemId.ToString() == itemId)
                        ?.Kills;
            }

            return characterAchievement.EarnedCount;
        }
    }
}

using System.Threading.Tasks;
using System;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.CensusStore.Services;
using AutoMapper;
using Voidwell.DaybreakGames.Domain.Models;
using System.Linq;
using Voidwell.DaybreakGames.Data.Repositories;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class DirectiveService : IDirectiveService
    {
        private readonly IDirectiveRepository _directiveRepository;
        private readonly ICharacterDirectiveStore _characterDirectiveStore;
        private readonly ICharacterService _characterService;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        private const string _cacheKey = "ps2.directive";
        private readonly Func<int, string> _getDirectivesOutlineCacheKey = (factionId) => $"{_cacheKey}_directive_outline_{factionId}";
        private readonly Func<string, string> _getDirectivesCharacterCacheKey = (characterId) => $"{_cacheKey}_character_{characterId}";

        private readonly TimeSpan _cacheDirectivesOutlineExpiration = TimeSpan.FromMinutes(60);
        private readonly TimeSpan _cacheDirectivesCharacterExpiration = TimeSpan.FromMinutes(10);

        public DirectiveService(IDirectiveRepository directiveRepository, ICharacterDirectiveStore characterDirectiveStore,
            ICharacterService characterService, ICache cache, IMapper mapper)
        {
            _directiveRepository = directiveRepository;
            _characterDirectiveStore = characterDirectiveStore;
            _characterService = characterService;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<DirectivesOutline> GetDirectivesOutlineAsync(int factionId)
        {
            var cacheKey = _getDirectivesOutlineCacheKey(factionId);

            var data = await _cache.GetAsync<DirectivesOutline>(cacheKey);
            if (data != null)
            {
                //return data;
            }

            var categories = await _directiveRepository.GetDirectiveTreesCategoriesAsync();

            data = _mapper.Map<DirectivesOutline>(categories);

            PopulateObjectiveTargets(data, categories);

            foreach (var tier in data.Categories.SelectMany(a => a.Trees).SelectMany(a => a.Tiers))
            {
                foreach (var directive in tier.Directives)
                {
                    directive.Objectives = directive.Objectives.Where(a => a.FactionId == -1 || a.FactionId == 0 || a.FactionId == 4 || a.FactionId == factionId);
                }

                tier.Directives = tier.Directives.Where(a => a.Objectives.Any()).ToList();
            }

            if (data != null)
            {
                await _cache.SetAsync(cacheKey, data, _cacheDirectivesOutlineExpiration);
            }

            return data;
        }

        public async Task<CharacterDirectivesOutline> GetCharacterDirectivesAsync(string characterId)
        {
            var cacheKey = _getDirectivesCharacterCacheKey(characterId);

            var data = await _cache.GetAsync<CharacterDirectivesOutline>(cacheKey);
            if (data != null)
            {
                //return data;
            }

            var characterTrees = await _characterDirectiveStore.GetCharacterDirectivesAsync(characterId);

            if (characterTrees == null || !characterTrees.Any())
            {
                return null;
            }

            data = _mapper.Map<CharacterDirectivesOutline>(characterTrees);

            await SetObjectiveAchievementProgress(characterId, data, characterTrees);

            if (data != null)
            {
                await _cache.SetAsync(cacheKey, data, _cacheDirectivesCharacterExpiration);
            }

            return data;
        }

        private void PopulateObjectiveTargets(DirectivesOutline outline, IEnumerable<DirectiveTreeCategory> categories)
        {
            var storeObjectives = categories.SelectMany(a => a.Trees).SelectMany(a => a.Tiers).SelectMany(a => a.Directives).SelectMany(a => a.ObjectiveSet.Objectives);
            var outlineObjectives = outline.Categories.SelectMany(a => a.Trees).SelectMany(a => a.Tiers).SelectMany(a => a.Directives).SelectMany(a => a.Objectives);

            foreach (var objective in storeObjectives)
            {
                string param = null;

                if (objective.ObjectiveTypeId == 66)
                {
                    if (int.TryParse(objective.Param1, out var achievementId))
                    {
                        if (objective.Achievement == null || objective.Achievement.Objective == null)
                        {
                            continue;
                        }

                        if (objective.Achievement.Objective.Param3 != null)
                        {
                            var outlineObjective = outlineObjectives.First(a => a.Id == objective.Id);
                            outlineObjective.GoalValue = int.Parse(objective.Param3);

                            continue;
                        }

                        param = objective.Param1;
                    }
                }
                else
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
                    }
                }

                if (int.TryParse(param, out var paramVal))
                {
                    var outlineObjective = outlineObjectives.First(a => a.Id == objective.Id);
                    outlineObjective.GoalValue = paramVal;
                }
            }
        }

        private async Task SetObjectiveAchievementProgress(string characterId, CharacterDirectivesOutline outline, IEnumerable<CharacterDirectiveTree> characterTrees)
        {
            var characterAchievementsTask = _characterService.GetCharacterAchievementsAsync(characterId);
            var characterDetailsTask = _characterService.GetCharacterDetails(characterId);

            await Task.WhenAll(characterAchievementsTask, characterDetailsTask);

            var characterAchievements = characterAchievementsTask.Result;
            var characterDetails = characterDetailsTask.Result;

            var outlineObjectives = outline.Trees.SelectMany(a => a.Directives).SelectMany(a => a.Objectives)
                .ToDictionary(a => a.Id, a => a);

            characterTrees
                .SelectMany(a => a.CharacterDirectives)
                .SelectMany(a => a.CharacterDirectiveObjectives)
                .Where(a => a.Objective.ObjectiveTypeId == 66 && a.Objective.Param1 != null)
                .ToList()
                .ForEach(a =>
                {
                    var value = GetAchievementProgress(a.Objective.Param1, characterDetails, characterAchievements);
                    if (value != null)
                    {
                        outlineObjectives[a.ObjectiveId].Progress = value;
                    }
                });
        }

        private int? GetAchievementProgress(string achievementId, CharacterDetails characterDetails, IEnumerable<CharacterAchievement> characterAchievements)
        {
            if (achievementId == null)
            {
                return null;
            }

            var characterAch = characterAchievements.FirstOrDefault(a => a.AchievementId.ToString() == achievementId);
            if (characterAch == null)
            {
                return null;
            }

            var achObjective = characterAch.Achievement.Objective;
            if (achObjective == null)
            {
                return null;
            }

            if (achObjective.ObjectiveTypeId == 12)
            {
                if (achObjective.Param5 != null)
                {
                    var weaponStat = characterDetails.WeaponStats.FirstOrDefault(a => a.ItemId.ToString() == achObjective.Param5);

                    return weaponStat?.Stats?.Kills;
                }

                return null;
            }
            else
            {
                if (characterAch != null)
                {
                    return characterAch.EarnedCount;
                }

                return null;
            }
        }
    }
}

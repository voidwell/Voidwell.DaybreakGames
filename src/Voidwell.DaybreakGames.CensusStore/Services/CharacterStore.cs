using DaybreakGames.Census.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.Microservice.Utility;
using AutoMapper;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class CharacterStore : ICharacterStore
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly CharacterCollection _characterCollection;
        private readonly CharacterNameCollection _characterNameCollection;
        private readonly CharactersStatCollection _charactersStatCollection;
        private readonly CharactersStatByFactionCollection _charactersStatByFactionCollection;
        private readonly CharactersWeaponStatCollection _charactersWeaponStatCollection;
        private readonly CharactersWeaponStatByFactionCollection _charactersWeaponStatByFactionCollection;
        private readonly CharactersStatHistoryCollection _charactersStatHistoryCollection;
        private readonly CharactersAchievementCollection _charactersAchievementCollection;
        private readonly IOutfitStore _outfitStore;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        private const string _cacheKey = "ps2.characterstore";
        private readonly Func<string, string> _getCharacterCacheKey = characterId => $"{_cacheKey}_character_{characterId}";
        private readonly Func<string, string> _getDetailsCacheKey = characterId => $"{_cacheKey}_details_{characterId}";
        private readonly Func<string, string> _getCharacterIdCacheKey = characterName => $"{_cacheKey}_name_{characterName.ToLower()}";
        private readonly Func<int, string> _getLeaderboardDataCacheKey = weaponId => $"{_cacheKey}_leaderboard_{weaponId}";

        private readonly TimeSpan _cacheCharacterExpiration = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _cacheCharacterIdExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _cacheWeaponLeaderboardDataExpiration = TimeSpan.FromMinutes(5);

        private readonly KeyedSemaphoreSlim _characterLock = new KeyedSemaphoreSlim();

        public CharacterStore(
            ICharacterRepository characterRepository,
            CharacterCollection characterCollection,
            CharacterNameCollection characterNameCollection,
            CharactersStatCollection charactersStatCollection,
            CharactersStatByFactionCollection characterStatByFactionCollection,
            CharactersWeaponStatCollection charactersWeaponStatCollection,
            CharactersWeaponStatByFactionCollection charactersWeaponStatByFactionCollection,
            CharactersStatHistoryCollection charactersStatHistoryCollection,
            CharactersAchievementCollection charactersAchievementCollection,
            IOutfitStore outfitStore,
            ICache cache,
            IMapper mapper)
        {
            _characterRepository = characterRepository;
            _characterCollection = characterCollection;
            _characterNameCollection = characterNameCollection;
            _charactersStatCollection = charactersStatCollection;
            _charactersStatByFactionCollection = characterStatByFactionCollection;
            _charactersWeaponStatCollection = charactersWeaponStatCollection;
            _charactersWeaponStatByFactionCollection = charactersWeaponStatByFactionCollection;
            _charactersStatHistoryCollection = charactersStatHistoryCollection;
            _charactersAchievementCollection = charactersAchievementCollection;
            _outfitStore = outfitStore;
            _cache = cache;
            _mapper = mapper;
        }

        public Task<IEnumerable<CensusCharacterModel>> LookupCharactersByName(string query, int limit = 12)
        {
            return _characterCollection.LookupCharactersByNameAsync(query, limit);
        }

        public Task<IEnumerable<Character>> FindCharacters(IEnumerable<string> characterIds)
        {
            return _characterRepository.GetCharactersByIdsAsync(characterIds);
        }

        public async Task<Character> GetCharacter(string characterId)
        {
            Character character;

            using (await _characterLock.WaitAsync(characterId))
            {
                var cacheKey = _getCharacterCacheKey(characterId);

                character = await _cache.GetAsync<Character>(cacheKey);
                if (character != null)
                {
                    return character;
                }

                character = await _characterRepository.GetCharacterAsync(characterId);
                if (character == null)
                {
                    try
                    {
                        character = await UpdateCharacter(characterId);
                    }
                    catch (CensusConnectionException)
                    {
                        return null;
                    }
                }

                if (character != null)
                {
                    await _cache.SetAsync(cacheKey, character, _cacheCharacterExpiration);
                }
            }

            return character;
        }

        public async Task UpdateAllCharacterInfo(string characterId, DateTime? lastLoginDate = null)
        {
            var character = await UpdateCharacter(characterId);
            if (character == null)
            {
                return;
            }

            await Task.WhenAll(UpdateCharacterTimes(characterId),
                               UpdateCharacterStats(characterId, lastLoginDate),
                               UpdateCharacterWeaponStats(characterId, lastLoginDate),
                               UpdateCharacterStatsHistory(characterId, lastLoginDate),
                               UpdateCharacterAchievementsAsync(characterId, lastLoginDate));

            var characterCacheKey = _getCharacterCacheKey(characterId); ;
            var detailsCacheKey = _getDetailsCacheKey(characterId);
            await Task.WhenAll(_cache.RemoveAsync(characterCacheKey), _cache.RemoveAsync(detailsCacheKey));
        }

        public async Task<Character> GetCharacterDetailsAsync(string characterId)
        {
            var characterDetailsTask = _characterRepository.GetCharacterWithDetailsAsync(characterId);
            var censusCharacterTimesTask = _characterCollection.GetCharacterTimesAsync(characterId);

            await Task.WhenAll(characterDetailsTask, censusCharacterTimesTask);

            var character = characterDetailsTask.Result;
            var censusTimes = censusCharacterTimesTask.Result;

            if (character?.Time != null && (censusTimes == null || character?.Time?.LastSaveDate == censusTimes?.LastSaveDate))
            {
                return character;
            }

            await UpdateAllCharacterInfo(characterId);

            return await _characterRepository.GetCharacterWithDetailsAsync(characterId);
        }

        public async Task<IEnumerable<Character>> GetCharacterDetailsAsync(IEnumerable<string> characterIds)
        {
            var characters = await _characterRepository.GetCharacterWithDetailsAsync(characterIds.ToArray());

            var completeDetails = characters.Where(a => a.Time != null).ToList();

            var missingCharacterIds = characterIds.Where(a => !characters.Any(b => b.Id == a)).ToList();

            if (missingCharacterIds.Any())
            {
                var updateTasks = missingCharacterIds.Select(a => UpdateAllCharacterInfo(a));

                await Task.WhenAll(updateTasks);

                var updatedCharacters = await _characterRepository.GetCharacterWithDetailsAsync(missingCharacterIds.ToArray());

                completeDetails.AddRange(updatedCharacters);
            }

            return completeDetails;
        }

        public async Task<IEnumerable<CharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string sort = null, string sortDir = null)
        {
            var cacheKey = _getLeaderboardDataCacheKey(weaponItemId);

            var data = await _cache.GetAsync<IEnumerable<CharacterWeaponStat>>(cacheKey);
            if (data == null)
            {
                data = await _characterRepository.GetCharacterWeaponLeaderboardAsync(weaponItemId, 0, 1000);
                await _cache.SetAsync(cacheKey, data, _cacheWeaponLeaderboardDataExpiration);
            }

            if (sort != null)
            {
                data = data?.OrderBy(d =>
                {
                    return sort switch
                    {
                        "kills" => (float?)d.Kills,
                        "vehiclekills" => (float?)d.VehicleKills,
                        "deaths" => (float?)d.Deaths,
                        "kdr" => d.Deaths > 0 ? (float?)d.Kills / (float?)d.Deaths : null,
                        "accuracy" => d.FireCount > 0 ? (float?)d.HitCount / (float?)d.FireCount : null,
                        "hsr" => d.Kills > 0 ? (float?)d.Headshots / (float?)d.Kills : null,
                        "kph" => d.PlayTime > 0 ? (float?)d.Kills / ((float?)d.PlayTime / 3600.0) : null,
                        _ => null,
                    };
                })?.ToList();

                if (sortDir == "desc")
                {
                    data = data?.Reverse();
                }
            }

            return data?.Skip(page * limit).Take(limit);
        }

        public async Task<string> GetCharacterIdByName(string characterName)
        {
            var cacheKey = _getCharacterIdCacheKey(characterName);

            var characterId = await _cache.GetAsync<string>(cacheKey);
            if (characterId != null)
            {
                return characterId;
            }

            characterId = await _characterRepository.GetCharacterIdByName(characterName) ?? await _characterNameCollection.GetCharacterIdByNameAsync(characterName);

            if (characterId != null)
            {
                await _cache.SetAsync(cacheKey, characterId, _cacheCharacterIdExpiration);
            }

            return characterId;
        }

        public async Task<OutfitMember> GetCharactersOutfitAsync(string characterId)
        {
            var character = await GetCharacter(characterId);
            if (character == null)
            {
                return null;
            }

            return await _outfitStore.UpdateCharacterOutfitMembershipAsync(character);
        }

        public Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(string characterId)
        {
            return _characterRepository.GetCharacterAchievementsAsync(characterId);
        }

        public Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(string characterId)
        {
            return _characterRepository.GetWeaponStatsAsync(characterId);
        }

        private async Task<Character> UpdateCharacter(string characterId)
        {
            var character = await _characterCollection.GetCharacterAsync(characterId);

            if (character == null)
            {
                return null;
            }

            var model = new Character
            {
                Id = character.CharacterId,
                Name = character.Name.First,
                FactionId = character.FactionId,
                WorldId = character.WorldId,
                BattleRank = character.BattleRank.Value,
                BattleRankPercentToNext = character.BattleRank.PercentToNext,
                CertsEarned = character.Certs.EarnedPoints,
                TitleId = character.TitleId,
                PrestigeLevel = character.PrestigeLevel
            };

            await _characterRepository.UpsertAsync(model);

            await _outfitStore.UpdateCharacterOutfitMembershipAsync(model);

            return model;
        }

        private async Task UpdateCharacterTimes(string characterId)
        {
            var times = await _characterCollection.GetCharacterTimesAsync(characterId);

            if (times == null)
            {
                return;
            }

            var dataModel = new CharacterTime
            {
                CharacterId = characterId,
                CreatedDate = times.CreationDate,
                LastLoginDate = times.LastLoginDate,
                LastSaveDate = times.LastSaveDate,
                MinutesPlayed = times.MinutesPlayed
            };

            await _characterRepository.UpsertAsync(dataModel);
        }

        private async Task UpdateCharacterStats(string characterId, DateTime? LastLoginDate)
        {
            var statsTask = _charactersStatCollection.GetCharacterStatsAsync(characterId, LastLoginDate);
            var statsByFactionTask = _charactersStatByFactionCollection.GetCharacterFactionStatsAsync(characterId, LastLoginDate);

            await Task.WhenAll(statsTask, statsByFactionTask);

            var stats = statsTask.Result;
            var statsByFaction = statsByFactionTask.Result;

            if (stats == null && statsByFaction == null)
            {
                return;
            }

            var statModels = new List<CharacterStat>();
            var lifetimeStatModel = new CharacterLifetimeStat
            {
                CharacterId = characterId
            };
            var statByFactionModels = new List<CharacterStatByFaction>();
            var lifetimeStatByFactionModel = new CharacterLifetimeStatByFaction
            {
                CharacterId = characterId
            };

            var statGroups = stats.GroupBy(a => a.ProfileId).ToList();
            var statByFactionGroups = statsByFaction.GroupBy(a => a.ProfileId).ToList();

            foreach (var group in statGroups)
            {
                if (group.Key == 0)
                {
                    foreach (var stat in group)
                    {
                        StatValueMapper.AssignStatValue(ref lifetimeStatModel, stat.StatName, stat.ValueForever);
                    }
                    continue;
                }

                var dbModel = new CharacterStat
                {
                    CharacterId = characterId,
                    ProfileId = group.Key
                };

                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref dbModel, stat.StatName, stat.ValueForever);
                }

                statModels.Add(dbModel);
            }

            foreach (var group in statByFactionGroups)
            {
                if (group.Key == 0)
                {
                    foreach (var stat in group)
                    {
                        StatValueMapper.AssignStatValue(ref lifetimeStatByFactionModel, stat.StatName, stat.ValueForeverVs, stat.ValueForeverNc, stat.ValueForeverTr);
                        StatValueMapper.AssignStatValue(ref lifetimeStatModel, stat.StatName, stat.ValueForeverVs + stat.ValueForeverNc + stat.ValueForeverTr);
                    }
                    continue;
                }

                var dbModel = new CharacterStatByFaction
                {
                    CharacterId = characterId,
                    ProfileId = group.Key
                };

                var statModel = statModels.SingleOrDefault(a => a.ProfileId == group.Key);
                if (statModel == null)
                {
                    statModel = new CharacterStat
                    {
                        CharacterId = characterId,
                        ProfileId = group.Key
                    };
                    statModels.Add(statModel);
                }

                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref dbModel, stat.StatName, stat.ValueForeverVs, stat.ValueForeverNc, stat.ValueForeverTr);
                    StatValueMapper.AssignStatValue(ref statModel, stat.StatName, stat.ValueForeverVs + stat.ValueForeverNc + stat.ValueForeverTr);
                }

                statByFactionModels.Add(dbModel);
            }

            await _characterRepository.UpsertAsync(lifetimeStatModel);
            await _characterRepository.UpsertAsync(lifetimeStatByFactionModel);
            await _characterRepository.UpsertRangeAsync(statModels);
            await _characterRepository.UpsertRangeAsync(statByFactionModels);
        }

        private async Task UpdateCharacterWeaponStats(string characterId, DateTime? LastLoginDate)
        {
            var characterWepStatsTask = _charactersWeaponStatCollection.GetCharacterWeaponStatsAsync(characterId, LastLoginDate);
            var characterWepStatsByFactionTask = _charactersWeaponStatByFactionCollection.GetCharacterWeaponStatsByFactionAsync(characterId, LastLoginDate);

            await Task.WhenAll(characterWepStatsTask, characterWepStatsByFactionTask);

            var wepStats = characterWepStatsTask.Result;
            var wepStatsByFaction = characterWepStatsByFactionTask.Result;

            if (wepStats == null && wepStatsByFaction == null)
            {
                return;
            }

            var statModels = new List<CharacterWeaponStat>();
            var statByFactionModels = new List<CharacterWeaponStatByFaction>();

            var statGroups = wepStats.GroupBy(a => new { a.ItemId, a.VehicleId }).ToList();
            var statByFactionGroups = wepStatsByFaction.GroupBy(a => new { a.ItemId, a.VehicleId }).ToList();

            foreach (var group in statGroups)
            {
                var dbModel = new CharacterWeaponStat
                {
                    CharacterId = characterId,
                    ItemId = group.Key.ItemId,
                    VehicleId = group.Key.VehicleId
                };

                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref dbModel, stat.StatName, stat.Value);
                }

                statModels.Add(dbModel);
            }

            foreach (var group in statByFactionGroups)
            {
                var dbModel = new CharacterWeaponStatByFaction
                {
                    CharacterId = characterId,
                    ItemId = group.Key.ItemId,
                    VehicleId = group.Key.VehicleId
                };

                var statModel = statModels.SingleOrDefault(a => a.ItemId == group.Key.ItemId && a.VehicleId == group.Key.VehicleId);
                if (statModel == null)
                {
                    statModel = new CharacterWeaponStat
                    {
                        CharacterId = characterId,
                        ItemId = group.Key.ItemId,
                        VehicleId = group.Key.VehicleId
                    };
                    statModels.Add(statModel);
                }

                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref dbModel, stat.StatName, stat.ValueVs, stat.ValueNc, stat.ValueTr);
                    StatValueMapper.AssignStatValue(ref statModel, stat.StatName, stat.ValueVs + stat.ValueNc + stat.ValueTr);
                }

                statByFactionModels.Add(dbModel);
            }

            await _characterRepository.UpsertRangeAsync(statModels);
            await _characterRepository.UpsertRangeAsync(statByFactionModels);
        }

        private async Task UpdateCharacterStatsHistory(string characterId, DateTime? lastLoginDate)
        {
            var statsHistory = await _charactersStatHistoryCollection.GetCharacterStatsHistoryAsync(characterId, lastLoginDate);
            if (statsHistory == null)
            {
                return;
            }

            var dataModels = statsHistory.Select(a =>
            {
                var day = new List<int> {
                    a.Day.d01, a.Day.d02, a.Day.d03, a.Day.d04, a.Day.d05, a.Day.d06, a.Day.d07, a.Day.d08, a.Day.d09, a.Day.d10, a.Day.d11, a.Day.d12,
                    a.Day.d13, a.Day.d14, a.Day.d15, a.Day.d16, a.Day.d17, a.Day.d18, a.Day.d19, a.Day.d20, a.Day.d21, a.Day.d22, a.Day.d23, a.Day.d24,
                    a.Day.d25, a.Day.d26, a.Day.d27, a.Day.d28, a.Day.d29, a.Day.d30, a.Day.d31
                };

                var month = new List<int> {
                    a.Month.m01, a.Month.m02, a.Month.m03, a.Month.m04, a.Month.m05, a.Month.m06, a.Month.m07, a.Month.m08, a.Month.m09, a.Month.m10, a.Month.m11, a.Month.m12
                };

                var week = new List<int> {
                    a.Week.w01, a.Week.w02, a.Week.w03, a.Week.w04, a.Week.w05, a.Week.w06, a.Week.w07, a.Week.w08, a.Week.w09, a.Week.w10, a.Week.w11, a.Week.w12, a.Week.w13
                };

                return new CharacterStatHistory
                {
                    CharacterId = a.CharacterId,
                    StatName = a.StatName,
                    AllTime = a.AllTime,
                    OneLifeMax = a.OneLifeMax,
                    Day = JToken.FromObject(day).ToString(),
                    Week = JToken.FromObject(week).ToString(),
                    Month = JToken.FromObject(month).ToString()
                };
            });

            await _characterRepository.UpsertRangeAsync(dataModels);
        }

        private async Task UpdateCharacterAchievementsAsync(string characterId, DateTime? lastLoginDate)
        {
            var achievements = await _charactersAchievementCollection.GetCharacterAchievementsAsync(characterId, lastLoginDate);
            if (achievements == null)
            {
                return;
            }

            var dataModels = _mapper.Map<IEnumerable<CharacterAchievement>>(achievements);

            await _characterRepository.UpsertRangeAsync(dataModels);
        }
    }
}

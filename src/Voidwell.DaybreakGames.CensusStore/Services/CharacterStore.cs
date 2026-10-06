using System.Text.Json;
using AutoMapper;
using DaybreakGames.Census.Exceptions;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services;

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

    public async Task<Character?> GetCharacter(string characterId)
    {
        try
        {
            return await _cache.GetOrSetIfNotNullAsync(
                _getCharacterCacheKey(characterId),
                async ct => await _characterRepository.GetCharacterAsync(characterId) ?? await UpdateCharacterAsync(characterId),
                _cacheCharacterExpiration);
        }
        catch (CensusConnectionException)
        {
            return null;
        }
    }

    public async Task UpdateAllCharacterInfo(string characterId, DateTime? lastLoginDate = null)
    {
        var character = await UpdateCharacterAsync(characterId);
        if (character == null)
        {
            return;
        }

        await Task.WhenAll(UpdateCharacterTimesAsync(characterId),
                           UpdateCharacterStatsAsync(characterId, lastLoginDate),
                           UpdateCharacterWeaponStatsAsync(characterId, lastLoginDate),
                           UpdateCharacterStatsHistoryAsync(characterId, lastLoginDate),
                           UpdateCharacterAchievementsAsync(characterId, lastLoginDate));

        var characterCacheKey = _getCharacterCacheKey(characterId);
        ;
        var detailsCacheKey = _getDetailsCacheKey(characterId);
        await Task.WhenAll(_cache.RemoveAsync(characterCacheKey), _cache.RemoveAsync(detailsCacheKey));
    }

    public async Task<Character?> GetCharacterDetailsAsync(string characterId)
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

    public async Task<IEnumerable<CharacterWeaponStat>?> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string? sort = null, string? sortDir = null)
    {
        var data = await _cache.GetOrSetAsync(
            _getLeaderboardDataCacheKey(weaponItemId),
            ct => _characterRepository.GetCharacterWeaponLeaderboardAsync(weaponItemId, 0, 1000),
            _cacheWeaponLeaderboardDataExpiration);

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

    public async Task<string?> GetCharacterIdByName(string characterName)
    {
        return await _cache.GetOrSetIfNotNullAsync(
            _getCharacterIdCacheKey(characterName),
            async ct => await _characterRepository.GetCharacterIdByName(characterName) ?? await _characterNameCollection.GetCharacterIdByNameAsync(characterName),
            _cacheCharacterIdExpiration);
    }

    public async Task<OutfitMember?> GetCharactersOutfitAsync(string characterId)
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

    private async Task<Character?> UpdateCharacterAsync(string characterId)
    {
        var character = await _characterCollection.GetCharacterAsync(characterId);

        if (character == null)
        {
            return null;
        }

        var model = new Character
        {
            Id = character.CharacterId,
            Name = character.Name!.First,
            FactionId = character.FactionId,
            WorldId = character.WorldId,
            BattleRank = character.BattleRank!.Value,
            BattleRankPercentToNext = character.BattleRank.PercentToNext,
            CertsEarned = character.Certs!.EarnedPoints,
            TitleId = character.TitleId,
            PrestigeLevel = character.PrestigeLevel
        };

        await _characterRepository.UpsertAsync(model);

        await _outfitStore.UpdateCharacterOutfitMembershipAsync(model);

        return model;
    }

    private async Task UpdateCharacterTimesAsync(string characterId)
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

    private async Task UpdateCharacterStatsAsync(string characterId, DateTime? LastLoginDate)
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

        var statGroups = stats!.GroupBy(a => a.ProfileId).ToList();
        var statByFactionGroups = statsByFaction.GroupBy(a => a.ProfileId).ToList();

        foreach (var group in statGroups)
        {
            if (group.Key == 0)
            {
                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref lifetimeStatModel, stat.StatName!, stat.ValueForever);
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
                StatValueMapper.AssignStatValue(ref dbModel, stat.StatName!, stat.ValueForever);
            }

            statModels.Add(dbModel);
        }

        foreach (var group in statByFactionGroups)
        {
            if (group.Key == 0)
            {
                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref lifetimeStatByFactionModel, stat.StatName!, stat.ValueForeverVs, stat.ValueForeverNc, stat.ValueForeverTr);
                    StatValueMapper.AssignStatValue(ref lifetimeStatModel, stat.StatName!, stat.ValueForeverVs + stat.ValueForeverNc + stat.ValueForeverTr);
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
                StatValueMapper.AssignStatValue(ref dbModel, stat.StatName!, stat.ValueForeverVs, stat.ValueForeverNc, stat.ValueForeverTr);
                StatValueMapper.AssignStatValue(ref statModel, stat.StatName!, stat.ValueForeverVs + stat.ValueForeverNc + stat.ValueForeverTr);
            }

            statByFactionModels.Add(dbModel);
        }

        await _characterRepository.UpsertAsync(lifetimeStatModel);
        await _characterRepository.UpsertAsync(lifetimeStatByFactionModel);
        await _characterRepository.UpsertRangeAsync(statModels);
        await _characterRepository.UpsertRangeAsync(statByFactionModels);
    }

    private async Task UpdateCharacterWeaponStatsAsync(string characterId, DateTime? LastLoginDate)
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

        var statGroups = wepStats!.GroupBy(a => new { a.ItemId, a.VehicleId }).ToList();
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
                StatValueMapper.AssignStatValue(ref dbModel, stat.StatName!, stat.Value);
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
                StatValueMapper.AssignStatValue(ref dbModel, stat.StatName!, stat.ValueVs, stat.ValueNc, stat.ValueTr);
                StatValueMapper.AssignStatValue(ref statModel, stat.StatName!, stat.ValueVs + stat.ValueNc + stat.ValueTr);
            }

            statByFactionModels.Add(dbModel);
        }

        await _characterRepository.UpsertRangeAsync(statModels);
        await _characterRepository.UpsertRangeAsync(statByFactionModels);
    }

    private async Task UpdateCharacterStatsHistoryAsync(string characterId, DateTime? lastLoginDate)
    {
        var statsHistory = await _charactersStatHistoryCollection.GetCharacterStatsHistoryAsync(characterId, lastLoginDate);
        if (statsHistory == null)
        {
            return;
        }

        var dataModels = statsHistory.Select(a =>
        {
            var day = new List<int> {
                a.Day!.D01, a.Day.D02, a.Day.D03, a.Day.D04, a.Day.D05, a.Day.D06, a.Day.D07, a.Day.D08, a.Day.D09, a.Day.D10, a.Day.D11, a.Day.D12,
                a.Day.D13, a.Day.D14, a.Day.D15, a.Day.D16, a.Day.D17, a.Day.D18, a.Day.D19, a.Day.D20, a.Day.D21, a.Day.D22, a.Day.D23, a.Day.D24,
                a.Day.D25, a.Day.D26, a.Day.D27, a.Day.D28, a.Day.D29, a.Day.D30, a.Day.D31
            };

            var month = new List<int> {
                a.Month!.M01, a.Month.M02, a.Month.M03, a.Month.M04, a.Month.M05, a.Month.M06, a.Month.M07, a.Month.M08, a.Month.M09, a.Month.M10, a.Month.M11, a.Month.M12
            };

            var week = new List<int> {
                a.Week!.W01, a.Week.W02, a.Week.W03, a.Week.W04, a.Week.W05, a.Week.W06, a.Week.W07, a.Week.W08, a.Week.W09, a.Week.W10, a.Week.W11, a.Week.W12, a.Week.W13
            };

            return new CharacterStatHistory
            {
                CharacterId = a.CharacterId,
                StatName = a.StatName,
                AllTime = a.AllTime,
                OneLifeMax = a.OneLifeMax,
                Day = JsonSerializer.Serialize(day),
                Week = JsonSerializer.Serialize(week),
                Month = JsonSerializer.Serialize(month)
            };
        });

        await _characterRepository.UpsertRangeAsync(dataModels);
    }

    public async Task UpdateCharacterAchievementsAsync(string characterId, DateTime? lastLoginDate = null)
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

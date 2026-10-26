using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Mappers;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class CharacterService : ICharacterService
{
    private readonly ICharacterStore _characterStore;
    private readonly ICache _cache;
    private readonly IWeaponAggregateService _weaponAggregateService;
    private readonly IGradeService _gradeService;
    private readonly IWeaponService _weaponService;

    private const string _cacheKey = "ps2.character";
    private readonly Func<long, string> _getDetailsCacheKey = characterId => $"{_cacheKey}_details_{characterId}";

    private readonly TimeSpan _cacheCharacterDetailsExpiration = TimeSpan.FromMinutes(30);

    private readonly SemaphoreSlim _characterStatsLock = new SemaphoreSlim(10);

    public CharacterService(ICharacterStore characterStore, ICache cache, IWeaponAggregateService weaponAggregateService,
        IGradeService gradeService, IWeaponService weaponService)
    {
        _characterStore = characterStore;
        _cache = cache;
        _weaponAggregateService = weaponAggregateService;
        _gradeService = gradeService;
        _weaponService = weaponService;
    }

    public Task<Character> GetCharacter(long characterId)
    {
        return _characterStore.GetCharacter(characterId)!;
    }

    public async Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12)
    {
        var characterList = await _characterStore.LookupCharactersByName(query, limit);
        return characterList.Select(DomainMapper.MapSearchResult);
    }

    public Task<IEnumerable<Character>> FindCharacters(IEnumerable<long> characterIds)
    {
        return _characterStore.FindCharacters(characterIds);
    }

    public async Task<CharacterDetails?> GetCharacterDetails(long characterId)
    {
        var details = await GetCharacterDetailsFromCacheAsync(characterId);

        if (details != null)
        {
            return details;
        }

        var characterTask = _characterStore.GetCharacterDetailsAsync(characterId);
        var sanctionedWeaponsTask = _weaponService.GetAllSanctionedWeaponIds();

        var character = await characterTask;
        var sanctionedWeaponIds = await sanctionedWeaponsTask;

        if (character == null)
        {
            return null;
        }

        var weaponIds = character.WeaponStats!.Where(a => a.ItemId != 0).Select(a => a.ItemId);
        var aggregates = await _weaponAggregateService.GetAggregates(weaponIds);

        details = CreateCharacterDetails(character, aggregates, sanctionedWeaponIds);

        await SaveCharacterDetailsInCacheAsync(details);

        return details;
    }

    public Task UpdateAllCharacterInfo(long characterId, DateTime? lastLoginDate = null)
    {
        return _characterStore.UpdateAllCharacterInfo(characterId, lastLoginDate);
    }

    public Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(long characterId)
    {
        return _characterStore.GetCharacterAchievementsAsync(characterId);
    }

    public Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(long characterId)
    {
        return _characterStore.GetWeaponStatsAsync(characterId);
    }

    private async Task<IEnumerable<CharacterDetails>?> GetCharacterDetailsAsync(IEnumerable<long> characterIds)
    {
        var cachedDetails = (await Task.WhenAll(characterIds.Select(GetCharacterDetailsFromCacheAsync))).Where(a => a != null).ToList();

        var cachedIds = cachedDetails.Select(a => a.Id).ToHashSet();
        var remainingCharacterIds = characterIds.Where(a => !cachedIds.Contains(a)).ToList();
        if (!remainingCharacterIds.Any())
        {
            return cachedDetails;
        }

        var details = new List<CharacterDetails>(cachedDetails);

        var charactersTask = _characterStore.GetCharacterDetailsAsync(remainingCharacterIds);
        var sanctionedWeaponsTask = _weaponService.GetAllSanctionedWeaponIds();

        var characters = await charactersTask;
        var sanctionedWeaponIds = await sanctionedWeaponsTask;

        var weaponIds = characters.SelectMany(a => a.WeaponStats!).Where(a => a.ItemId != 0).Select(a => a.ItemId).Distinct().ToList();
        var aggregates = await _weaponAggregateService.GetAggregates(weaponIds);

        var charactersDetails = characters.Select(character => CreateCharacterDetails(character, aggregates, sanctionedWeaponIds)).ToList();

        await Task.WhenAll(charactersDetails.Select(SaveCharacterDetailsInCacheAsync));

        details.AddRange(charactersDetails);

        return details;
    }

    private Task<CharacterDetails> GetCharacterDetailsFromCacheAsync(long characterId)
    {
        var cacheKey = _getDetailsCacheKey(characterId);
        return _cache.GetAsync<CharacterDetails>(cacheKey);
    }

    private Task SaveCharacterDetailsInCacheAsync(CharacterDetails details)
    {
        var cacheKey = _getDetailsCacheKey(details.Id!.Value);
        return _cache.SetAsync(cacheKey, details, _cacheCharacterDetailsExpiration);
    }

    private static CharacterDetails CreateCharacterDetails(Character character, Dictionary<string, WeaponAggregate> aggregates, IEnumerable<int> sanctionedWeaponIds)
    {
        var details = DomainMapper.MapCharacterDetails(character);

        details.WeaponStats = CalculateCharacterWeaponStats(character.WeaponStats!, aggregates).ToList();
        details.InfantryStats = CalculateInfantryStats(details.WeaponStats, details.LifetimeStats!, sanctionedWeaponIds);

        return details;
    }

    private static double? Ratio(double? numerator, double? denominator)
        => denominator > 0 ? numerator / denominator : null;

    private static double? ZScore(double? value, double? average, double? standardDeviation)
        => standardDeviation > 0 ? (value - average) / standardDeviation : null;

    private static IEnumerable<CharacterDetailsWeaponStat> CalculateCharacterWeaponStats(IEnumerable<CharacterWeaponStat> weaponStats, Dictionary<string, WeaponAggregate> aggregates)
    {
        foreach (var s in weaponStats.Where(a => a.ItemId != 0))
        {
            var stat = DomainMapper.MapWeaponStat(s);
            var st = stat.Stats!;

            var kills = s.Kills!.Value;
            var playTime = s.PlayTime!.Value;

            st.Accuracy = Ratio(s.HitCount!.Value, s.FireCount!.Value);
            st.HeadshotRatio = Ratio(s.Headshots!.Value, kills);
            st.LandedPerKill = Ratio(s.HitCount.Value, kills);
            st.ShotsPerKill = Ratio(s.FireCount.Value, kills);
            st.KillDeathRatio = Ratio(kills, s.Deaths!.Value);
            st.KillsPerHour = Ratio(kills, playTime / 3600.0);
            st.ScorePerMinute = Ratio(s.Score!.Value, playTime / 60.0);
            st.VehicleKillsPerHour = Ratio(s.VehicleKills!.Value, playTime / 3600.0);

            if (aggregates != null && aggregates.TryGetValue($"{s.ItemId}-{s.VehicleId}", out var agg))
            {
                st.KillDeathRatioDelta = ZScore(st.KillDeathRatio, agg.AVGKdr, agg.STDKdr);
                st.AccuracyDelta = ZScore(st.Accuracy, agg.AVGAccuracy, agg.STDAccuracy);
                st.HsrDelta = ZScore(st.HeadshotRatio, agg.AVGHsr, agg.STDHsr);
                st.KphDelta = ZScore(st.KillsPerHour, agg.AVGKph, agg.STDKph);
                st.VehicleKphDelta = ZScore(st.VehicleKillsPerHour, agg.AVGVkph, agg.STDVkph);
            }

            yield return stat;
        }
    }

    private static InfantryStats? CalculateInfantryStats(IEnumerable<CharacterDetailsWeaponStat> weaponStats, CharacterDetailsLifetimeStats lifetimeStats, IEnumerable<int> sanctionedWeaponIds)
    {
        if (sanctionedWeaponIds == null)
        {
            return null;
        }

        var sanctionedIds = sanctionedWeaponIds.ToHashSet();
        var weaponStatList = weaponStats.ToList();

        var allSanctionedWeaponsDeathsSum = weaponStatList.Where(a => sanctionedIds.Contains(a.ItemId)).Sum(a => a.Stats!.Deaths);

        var sanctionedWeapons = weaponStatList.Where(a => a.Stats?.Kills >= 50 && sanctionedIds.Contains(a.ItemId)).ToList();

        var sumHitCount = sanctionedWeapons.Sum(a => a.Stats!.HitCount);
        var sumFireCount = sanctionedWeapons.Sum(a => a.Stats!.FireCount);
        var sumKills = sanctionedWeapons.Sum(a => a.Stats!.Kills);
        var sumHeadshots = sanctionedWeapons.Sum(a => a.Stats!.Headshots);
        var sumPlayTime = sanctionedWeapons.Sum(a => a.Stats!.PlayTime);

        var infantryStats = new InfantryStats
        {
            Weapons = sanctionedWeapons.Count,
            UnsanctionedWeapons = weaponStatList.Count - sanctionedWeapons.Count,
            Kills = sumKills,
            Accuracy = Ratio(sumHitCount, sumFireCount),
            HeadshotRatio = Ratio(sumHeadshots, sumKills),
            KillDeathRatio = Ratio(sumKills, allSanctionedWeaponsDeathsSum),
            KillsPerMinute = Ratio(sumKills, sumPlayTime / 60.0),
            AccuracyDelta = sanctionedWeapons.Average(a => a.Stats!.AccuracyDelta),
            HeadshotRatioDelta = sanctionedWeapons.Average(a => a.Stats!.HsrDelta),
            KillDeathRatioDelta = sanctionedWeapons.Average(a => a.Stats!.KillDeathRatioDelta),
            KillsPerMinuteDelta = sanctionedWeapons.Average(a => a.Stats!.KphDelta)
        };

        infantryStats.IVIScore = (int)Math.Round(infantryStats.HeadshotRatio.GetValueOrDefault() * infantryStats.Accuracy.GetValueOrDefault() * 10000, 0);

        if (lifetimeStats.Deaths > 0)
        {
            infantryStats.KDRPadding = lifetimeStats.Kills / (double)lifetimeStats.Deaths - infantryStats.KillDeathRatio.GetValueOrDefault();
        }

        return infantryStats;
    }

    public Task<OutfitMember> GetCharactersOutfit(long characterId)
    {
        return _characterStore.GetCharactersOutfitAsync(characterId)!;
    }

    public async Task<SimpleCharacterDetails?> GetCharacterByName(string characterName)
    {
        try
        {
            await _characterStatsLock.WaitAsync();

            var character = await GetCharacterDetailsByNameAsync(characterName);
            if (character == null)
            {
                return null;
            }

            return MapToSimpleCharacterDetails(character);
        }
        finally
        {
            _characterStatsLock.Release();
        }
    }

    public async Task<IEnumerable<SimpleCharacterDetails>?> GetCharactersByName(IEnumerable<string> characterNames)
    {
        var characters = await GetCharacterDetailsByNameAsync(characterNames);
        if (characters == null)
        {
            return null;
        }
        return characters.Select(MapToSimpleCharacterDetails).ToList();
    }

    private static SimpleCharacterDetails MapToSimpleCharacterDetails(CharacterDetails character)
    {
        var mostPlayedWeapon = character.WeaponStats!.OrderByDescending(a => a.Stats!.Kills)
                .FirstOrDefault();
        var mostPlayedProfile = character.ProfileStats!.Where(a => a.ProfileId != 0)
            .OrderByDescending(a => a.PlayTime)
            .FirstOrDefault();

        var details = DomainMapper.MapSimpleDetails(character);

        details.MostPlayedWeaponName = mostPlayedWeapon?.Name;
        details.MostPlayedWeaponId = mostPlayedWeapon?.ItemId;
        details.MostPlayedWeaponKills = mostPlayedWeapon?.Stats!.Kills.GetValueOrDefault();
        details.MostPlayedClassName = mostPlayedProfile?.ProfileName;
        details.MostPlayedClassId = mostPlayedProfile?.ProfileId;

        details.KillDeathRatio = (double)details.Kills / details.Deaths;
        details.HeadshotRatio = (double)character.LifetimeStats!.Headshots / details.Kills;
        details.KillsPerHour = details.Kills / (details.PlayTime / 3600.0);
        details.TotalKillsPerHour = details.Kills / (details.TotalPlayTimeMinutes / 60.0);
        details.SiegeLevel = (double)character.LifetimeStats.FacilityCaptureCount / character.LifetimeStats.FacilityDefendedCount * 100;

        return details;
    }

    public async Task<CharacterWeaponDetails?> GetCharacterWeaponByName(string characterName, string weaponName)
    {
        var character = await GetCharacterDetailsByNameAsync(characterName);
        if (character == null)
        {
            return null;
        }

        var weaponStats = FindCharacterWeaponStatsByNameOrId(character.WeaponStats!, weaponName);
        if (weaponStats == null)
        {
            return null;
        }

        var details = new CharacterWeaponDetails
        {
            CharacterId = character.Id,
            CharacterName = character.Name,
            ItemId = weaponStats.ItemId,
            WeaponName = weaponStats.Name,
            WeaponImageId = weaponStats.ImageId,
            Kills = weaponStats.Stats!.Kills,
            Deaths = weaponStats.Stats.Deaths,
            Headshots = weaponStats.Stats.Headshots,
            Score = weaponStats.Stats.Score,
            PlayTime = weaponStats.Stats.PlayTime,
            Accuracy = weaponStats.Stats.Accuracy,
            HeadshotRatio = weaponStats.Stats.HeadshotRatio,
            KillDeathRatio = weaponStats.Stats.KillDeathRatio,
            KillsPerHour = weaponStats.Stats.KillsPerHour,

            AccuracyGrade = _gradeService.GetGradeByDelta(weaponStats.Stats.AccuracyDelta),
            HeadshotRatioGrade = _gradeService.GetGradeByDelta(weaponStats.Stats.HsrDelta),
            KillDeathRatioGrade = _gradeService.GetGradeByDelta(weaponStats.Stats.KillDeathRatioDelta),
            KillsPerHourGrade = _gradeService.GetGradeByDelta(weaponStats.Stats.KphDelta)
        };

        return details;
    }

    private async Task<IEnumerable<CharacterDetails>?> GetCharacterDetailsByNameAsync(IEnumerable<string> characterNames)
    {
        var characterIdResults = await Task.WhenAll(characterNames.Select(_characterStore.GetCharacterIdByName));
        var characterIds = characterIdResults.OfType<long>();

        if (!characterIds.Any())
        {
            return null;
        }

        return await GetCharacterDetailsAsync(characterIds);
    }

    private async Task<CharacterDetails?> GetCharacterDetailsByNameAsync(string characterName)
    {
        var characterId = await _characterStore.GetCharacterIdByName(characterName);
        if (characterId == null)
        {
            return null;
        }

        return await GetCharacterDetails(characterId.Value);
    }

    private static CharacterDetailsWeaponStat? FindCharacterWeaponStatsByNameOrId(IEnumerable<CharacterDetailsWeaponStat> stats, string weaponSearch)
    {
        CharacterDetailsWeaponStat weaponStats = null!;

        var validStats = stats.Where(a => a.Name != null && (a.Stats!.Kills.GetValueOrDefault() > 0 || a.Stats.PlayTime.GetValueOrDefault() > 0)).ToList();

        if (int.TryParse(weaponSearch, out var weaponId))
        {
            weaponStats = validStats.FirstOrDefault(a => a.ItemId == weaponId)!;
        }

        return weaponStats ?? validStats
                   .OrderByDescending(a => a.Stats!.Kills)
                   .ThenBy(a => a.Name)
                   .FirstOrDefault(a => a.Name!.ToLower().Contains(weaponSearch.ToLower()));
    }
}

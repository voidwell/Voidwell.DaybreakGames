using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;
using System;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Microservice.Cache;
using System.Threading;
using Voidwell.DaybreakGames.CensusStore.Services;
using AutoMapper;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterStore _characterStore;
        private readonly ICache _cache;
        private readonly IWeaponAggregateService _weaponAggregateService;
        private readonly IGradeService _gradeService;
        private readonly IWeaponService _weaponService;
        private readonly IMapper _mapper;

        private const string _cacheKey = "ps2.character";
        private readonly Func<string, string> _getDetailsCacheKey = characterId => $"{_cacheKey}_details_{characterId}";

        private readonly TimeSpan _cacheCharacterDetailsExpiration = TimeSpan.FromMinutes(30);

        private readonly SemaphoreSlim _characterStatsLock = new SemaphoreSlim(10);

        public CharacterService(ICharacterStore characterStore, ICache cache, IWeaponAggregateService weaponAggregateService,
            IGradeService gradeService, IWeaponService weaponService, IMapper mapper)
        {
            _characterStore = characterStore;
            _cache = cache;
            _weaponAggregateService = weaponAggregateService;
            _gradeService = gradeService;
            _weaponService = weaponService;
            _mapper = mapper;
        }

        public Task<Character> GetCharacter(string characterId)
        {
            return _characterStore.GetCharacter(characterId);
        }

        public async Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12)
        {
            var characterList = await _characterStore.LookupCharactersByName(query, limit);
            return characterList.Select(_mapper.Map<CharacterSearchResult>);
        }

        public Task<IEnumerable<Character>> FindCharacters(IEnumerable<string> characterIds)
        {
            return _characterStore.FindCharacters(characterIds);
        }

        public async Task<CharacterDetails> GetCharacterDetails(string characterId)
        {
            var details = await GetCharacterDetailsFromCacheAsync(characterId);

            if (details != null)
            {
                return details;
            }

            var characterTask = _characterStore.GetCharacterDetailsAsync(characterId);
            var sanctionedWeaponsTask = _weaponService.GetAllSanctionedWeaponIds();

            await Task.WhenAll(characterTask, sanctionedWeaponsTask);

            var character = characterTask.Result;
            var sanctionedWeaponIds = sanctionedWeaponsTask.Result;

            if (character == null)
            {
                return null;
            }

            var weaponIds = character.WeaponStats.Where(a => a.ItemId != 0).Select(a => a.ItemId);
            var aggregates = await _weaponAggregateService.GetAggregates(weaponIds);

            details = await CreateCharacterDetailsAsync(character, aggregates, sanctionedWeaponIds);

            await SaveCharacterDetailsInCacheAsync(details);

            return details;
        }

        public Task UpdateAllCharacterInfo(string characterId, DateTime? lastLoginDate = null)
        {
            return _characterStore.UpdateAllCharacterInfo(characterId, lastLoginDate);
        }

        public Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(string characterId)
        {
            return _characterStore.GetCharacterAchievementsAsync(characterId);
        }

        public Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(string characterId)
        {
            return _characterStore.GetWeaponStatsAsync(characterId);
        }

        private async Task<IEnumerable<CharacterDetails>> GetCharacterDetails(IEnumerable<string> characterIds)
        {
            var cachedDetails = (await Task.WhenAll(characterIds.Select(GetCharacterDetailsFromCacheAsync))).Where(a => a != null);

            var remainingCharacterIds = characterIds.Where(a => cachedDetails.All(b => b.Id != a));
            if (!remainingCharacterIds.Any())
            {
                return cachedDetails;
            }

            var details = new List<CharacterDetails>();
            details.AddRange(cachedDetails);

            var charactersTask = _characterStore.GetCharacterDetailsAsync(remainingCharacterIds);
            var sanctionedWeaponsTask = _weaponService.GetAllSanctionedWeaponIds();

            await Task.WhenAll(charactersTask, sanctionedWeaponsTask);

            var characters = charactersTask.Result;
            var sanctionedWeaponIds = sanctionedWeaponsTask.Result;

            var weaponIds = characters.SelectMany(a => a.WeaponStats).Where(a => a.ItemId != 0).Select(a => a.ItemId).Distinct().ToList();
            var aggregates = await _weaponAggregateService.GetAggregates(weaponIds);

            var characterDetailMappingTasks = characters.Select(character => CreateCharacterDetailsAsync(character, aggregates, sanctionedWeaponIds));
            var charactersDetails = await Task.WhenAll(characterDetailMappingTasks);

            await Task.WhenAll(charactersDetails.Select(SaveCharacterDetailsInCacheAsync));

            details.AddRange(charactersDetails);

            return details;
        }

        private Task<CharacterDetails> GetCharacterDetailsFromCacheAsync(string characterId)
        {
            var cacheKey = _getDetailsCacheKey(characterId);
            return _cache.GetAsync<CharacterDetails>(cacheKey);
        }

        private Task SaveCharacterDetailsInCacheAsync(CharacterDetails details)
        {
            var cacheKey = _getDetailsCacheKey(details.Id);
            return _cache.SetAsync(cacheKey, details, _cacheCharacterDetailsExpiration);
        }

        private Task<CharacterDetails> CreateCharacterDetailsAsync(Character character, Dictionary<string, WeaponAggregate> aggregates, IEnumerable<int> sanctionedWeaponIds)
        {
            var details = _mapper.Map<CharacterDetails>(character);

            details.WeaponStats = CalculateCharacterWeaponStats(character.WeaponStats, aggregates);
            details.InfantryStats = CalculateInfantryStats(details.WeaponStats, details.LifetimeStats, sanctionedWeaponIds);

            return Task.FromResult(details);
        }

        private IEnumerable<CharacterDetailsWeaponStat> CalculateCharacterWeaponStats(IEnumerable<CharacterWeaponStat> weaponStats, Dictionary<string, WeaponAggregate> aggregates)
        {
            foreach(var s in weaponStats?.Where(a => a.ItemId != 0))
            {
                var stat = _mapper.Map<CharacterDetailsWeaponStat>(s);

                if (s.FireCount.Value > 0)
                {
                    stat.Stats.Accuracy = (double)s.HitCount.Value / s.FireCount.Value;
                }

                if (s.Kills.Value > 0)
                {
                    stat.Stats.HeadshotRatio = (double)s.Headshots.Value / s.Kills.Value;
                    stat.Stats.LandedPerKill = (double)s.HitCount.Value / s.Kills.Value;
                    stat.Stats.ShotsPerKill = (double)s.FireCount.Value / s.Kills.Value;
                }

                if (s.Deaths.Value > 0)
                {
                    stat.Stats.KillDeathRatio = (double)s.Kills.Value / s.Deaths.Value;
                }

                if (s.PlayTime.Value > 0)
                {
                    stat.Stats.KillsPerHour = s.Kills.Value / (s.PlayTime.Value / 3600.0);
                    stat.Stats.ScorePerMinute = s.Score.Value / (s.PlayTime.Value / 60.0);
                    stat.Stats.VehicleKillsPerHour = s.VehicleKills.Value / (s.PlayTime.Value / 3600.0);
                }

                if (aggregates != null && aggregates.TryGetValue($"{s.ItemId}-{s.VehicleId}", out var agg))
                {
                    if (stat.Stats.KillDeathRatio.HasValue && agg.STDKdr > 0)
                    {
                        stat.Stats.KillDeathRatioDelta = (stat.Stats.KillDeathRatio - agg.AVGKdr) / agg.STDKdr;
                    }

                    if (stat.Stats.Accuracy.HasValue && agg.STDAccuracy > 0)
                    {
                        stat.Stats.AccuracyDelta = (stat.Stats.Accuracy - agg.AVGAccuracy) / agg.STDAccuracy;
                    }

                    if (stat.Stats.HeadshotRatio.HasValue && agg.STDHsr > 0)
                    {
                        stat.Stats.HsrDelta = (stat.Stats.Accuracy - agg.AVGHsr) / agg.STDHsr;
                    }

                    if (stat.Stats.KillsPerHour.HasValue && agg.STDKph > 0)
                    {
                        stat.Stats.KphDelta = (stat.Stats.KillsPerHour - agg.AVGKph) / agg.STDKph;
                    }

                    if (stat.Stats.VehicleKillsPerHour.HasValue && agg.STDVkph > 0)
                    {
                        stat.Stats.VehicleKphDelta = (stat.Stats.VehicleKillsPerHour - agg.AVGVkph) / agg.STDVkph;
                    }
                }

                yield return stat;
            };
        }

        private static InfantryStats CalculateInfantryStats(IEnumerable<CharacterDetailsWeaponStat> weaponStats, CharacterDetailsLifetimeStats lifetimeStats, IEnumerable<int> sanctionedWeaponIds)
        {
            if (sanctionedWeaponIds == null)
            {
                return null;
            }

            var allSanctionedWeapons = weaponStats.Where(a => sanctionedWeaponIds.Contains(a.ItemId)).ToList();
            var allSanctionedWeaponsDeathsSum = allSanctionedWeapons.Sum(a => a.Stats.Deaths);

            var sanctionedWeapons = weaponStats.Where(a => a.Stats?.Kills >= 50).Where(a => sanctionedWeaponIds.Contains(a.ItemId)).ToList();
            var unSanctionedWeapons = weaponStats.Where(a => !sanctionedWeapons.Contains(a)).ToList();

            var sumHitCount = sanctionedWeapons.Sum(a => a.Stats.HitCount);
            var sumFireCount = sanctionedWeapons.Sum(a => a.Stats.FireCount);
            var sumKills = sanctionedWeapons.Sum(a => a.Stats.Kills);
            var sumHeadshots = sanctionedWeapons.Sum(a => a.Stats.Headshots);
            var sumPlayTime = sanctionedWeapons.Sum(a => a.Stats.PlayTime);

            var infantryStats = new InfantryStats
            {
                Weapons = sanctionedWeapons.Count,
                UnsanctionedWeapons = unSanctionedWeapons.Count,
                Kills = sumKills,
                AccuracyDelta = sanctionedWeapons.Average(a => a.Stats.AccuracyDelta),
                HeadshotRatioDelta = sanctionedWeapons.Average(a => a.Stats.HsrDelta),
                KillDeathRatioDelta = sanctionedWeapons.Average(a => a.Stats.KillDeathRatioDelta),
                KillsPerMinuteDelta = sanctionedWeapons.Average(a => a.Stats.KphDelta)
            };

            if (sumFireCount > 0)
            {
                infantryStats.Accuracy = sumHitCount / (double)sumFireCount;
            }

            if (sumKills > 0)
            {
                infantryStats.HeadshotRatio = sumHeadshots / (double)sumKills;
            }

            if (allSanctionedWeaponsDeathsSum > 0)
            {
                infantryStats.KillDeathRatio = sanctionedWeapons.Sum(a => a.Stats.Kills) / (double)allSanctionedWeaponsDeathsSum;
            }

            if (sumPlayTime > 0)
            {
                infantryStats.KillsPerMinute = sumKills / ((double)sumPlayTime / 60);
            }

            infantryStats.IVIScore = (int)Math.Round(infantryStats.HeadshotRatio.GetValueOrDefault() * 100 * (infantryStats.Accuracy.GetValueOrDefault() * 100), 0);

            if (lifetimeStats.Deaths > 0 && infantryStats != null)
            {
                infantryStats.KDRPadding = lifetimeStats.Kills / (double)lifetimeStats.Deaths - infantryStats.KillDeathRatio.GetValueOrDefault();
            }

            return infantryStats;
        }

        public Task<OutfitMember> GetCharactersOutfit(string characterId)
        {
            return _characterStore.GetCharactersOutfitAsync(characterId);
        }

        public async Task<SimpleCharacterDetails> GetCharacterByName(string characterName)
        {
            try
            {
                await _characterStatsLock.WaitAsync();

                var character = await GetCharacterDetailsByNameAsync(characterName);
                if (character == null)
                {
                    return null;
                }

                return await MapToSimpleCharacterDetailsAsync(character);
            }
            finally
            {
                _characterStatsLock.Release();
            }
        }

        public async Task<IEnumerable<SimpleCharacterDetails>> GetCharactersByName(IEnumerable<string> characterNames)
        {
            var characters = await GetCharacterDetailsByNameAsync(characterNames);
            if (characters == null)
            {
                return null;
            }
            return await Task.WhenAll(characters.Select(MapToSimpleCharacterDetailsAsync));
        }

        private Task<SimpleCharacterDetails> MapToSimpleCharacterDetailsAsync(CharacterDetails character)
        {
            var mostPlayedWeapon = character.WeaponStats.OrderByDescending(a => a.Stats.Kills)
                    .FirstOrDefault();
            var mostPlayedProfile = character.ProfileStats.Where(a => a.ProfileId != 0)
                .OrderByDescending(a => a.PlayTime)
                .FirstOrDefault();

            var details = _mapper.Map<SimpleCharacterDetails>(character);

            details.MostPlayedWeaponName = mostPlayedWeapon?.Name;
            details.MostPlayedWeaponId = mostPlayedWeapon?.ItemId;
            details.MostPlayedWeaponKills = mostPlayedWeapon?.Stats.Kills.GetValueOrDefault();
            details.MostPlayedClassName = mostPlayedProfile?.ProfileName;
            details.MostPlayedClassId = mostPlayedProfile?.ProfileId;

            details.KillDeathRatio = (double)details.Kills / details.Deaths;
            details.HeadshotRatio = (double)character.LifetimeStats.Headshots / details.Kills;
            details.KillsPerHour = details.Kills / (details.PlayTime / 3600.0);
            details.TotalKillsPerHour = details.Kills / (details.TotalPlayTimeMinutes / 60.0);
            details.SiegeLevel = (double)character.LifetimeStats.FacilityCaptureCount / character.LifetimeStats.FacilityDefendedCount * 100;

            return Task.FromResult(details);
        }

        public async Task<CharacterWeaponDetails> GetCharacterWeaponByName(string characterName, string weaponName)
        {
            var character = await GetCharacterDetailsByNameAsync(characterName);
            if (character == null)
            {
                return null;
            }

            var weaponStats = FindCharacterWeaponStatsByNameOrId(character.WeaponStats, weaponName);
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
                Kills = weaponStats.Stats.Kills,
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

        private async Task<IEnumerable<CharacterDetails>> GetCharacterDetailsByNameAsync(IEnumerable<string> characterNames)
        {
            var characterIdResults = await Task.WhenAll(characterNames.Select(_characterStore.GetCharacterIdByName));
            var characterIds = characterIdResults.Where(a => !string.IsNullOrWhiteSpace(a));

            if (!characterIds.Any())
            {
                return null;
            }

            return await GetCharacterDetails(characterIds);
        }

        private async Task<CharacterDetails> GetCharacterDetailsByNameAsync(string characterName)
        {
            var characterId = await _characterStore.GetCharacterIdByName(characterName);
            if (characterId == null)
            {
                return null;
            }

            return await GetCharacterDetails(characterId);
        }

        private static CharacterDetailsWeaponStat FindCharacterWeaponStatsByNameOrId(IEnumerable<CharacterDetailsWeaponStat> stats, string weaponSearch)
        {
            CharacterDetailsWeaponStat weaponStats = null;

            var validStats = stats.Where(a => a.Name != null && (a.Stats.Kills.GetValueOrDefault() > 0 || a.Stats.PlayTime.GetValueOrDefault() > 0)).ToList();

            if (int.TryParse(weaponSearch, out var weaponId))
            {
                weaponStats = validStats.FirstOrDefault(a => a.ItemId == weaponId);
            }

            return weaponStats ?? validStats
                       .OrderByDescending(a => a.Stats.Kills)
                       .ThenBy(a => a.Name)
                       .FirstOrDefault(a => a.Name.ToLower().Contains(weaponSearch.ToLower()));
        }
    }
}

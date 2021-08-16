using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using System;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Cache;
using Newtonsoft.Json.Linq;
using System.Threading;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.CensusStore.Services;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterStore _characterStore;
        private readonly ICache _cache;
        private readonly IWeaponAggregateService _weaponAggregateService;
        private readonly IGradeService _gradeService;
        private readonly IWeaponService _weaponService;

        private const string _cacheKey = "ps2.character";
        private readonly Func<string, string> _getDetailsCacheKey = characterId => $"{_cacheKey}_details_{characterId}";

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

        public Task<Character> GetCharacter(string characterId)
        {
            return _characterStore.GetCharacter(characterId);
        }

        public async Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12)
        {
            var characterList = await _characterStore.LookupCharactersByName(query, limit);
            return characterList.Select(CharacterSearchResult.LoadFromCensusCharacter);
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

        private static Task<CharacterDetails> CreateCharacterDetailsAsync(Character character, Dictionary<string, WeaponAggregate> aggregates, IEnumerable<int> sanctionedWeaponIds)
        {
            var times = new CharacterDetailsTimes
            {
                CreatedDate = character.Time.CreatedDate,
                LastLoginDate = character.Time.LastLoginDate,
                LastSaveDate = character.Time.LastSaveDate,
                MinutesPlayed = character.Time.MinutesPlayed
            };

            var lifetimeStats = new CharacterDetailsLifetimeStats
            {
                AchievementCount = character.LifetimeStats.AchievementCount.GetValueOrDefault(),
                AssistCount = character.LifetimeStats.AssistCount.GetValueOrDefault(),
                DominationCount = character.LifetimeStats.DominationCount.GetValueOrDefault(),
                FacilityCaptureCount = character.LifetimeStats.FacilityCaptureCount.GetValueOrDefault(),
                FacilityDefendedCount = character.LifetimeStats.FacilityDefendedCount.GetValueOrDefault(),
                MedalCount = character.LifetimeStats.MedalCount.GetValueOrDefault(),
                RevengeCount = character.LifetimeStats.RevengeCount.GetValueOrDefault(),
                SkillPoints = character.LifetimeStats.SkillPoints.GetValueOrDefault(),
                Kills = character.LifetimeStats.WeaponKills.GetValueOrDefault(),
                PlayTime = character.LifetimeStats.WeaponPlayTime.GetValueOrDefault(),
                VehicleKills = character.LifetimeStats.WeaponVehicleKills.GetValueOrDefault(),
                DamageGiven = character.LifetimeStats.WeaponDamageGiven.GetValueOrDefault(),
                DamageTakenBy = character.LifetimeStats.WeaponDamageTakenBy.GetValueOrDefault(),
                FireCount = character.LifetimeStats.WeaponFireCount.GetValueOrDefault(),
                HitCount = character.LifetimeStats.WeaponHitCount.GetValueOrDefault(),
                Score = character.LifetimeStats.WeaponScore.GetValueOrDefault(),
                Deaths = character.LifetimeStats.WeaponDeaths.GetValueOrDefault(),
                Headshots = character.LifetimeStats.WeaponHeadshots.GetValueOrDefault()
            };

            var profileStats = character.Stats?.Select(s => new CharacterDetailsProfileStat
            {
                ProfileId = s.ProfileId,
                ProfileName = s.Profile?.Name,
                ImageId = s.Profile?.ImageId,
                Deaths = s.Deaths.GetValueOrDefault(),
                FireCount = s.FireCount.GetValueOrDefault(),
                HitCount = s.HitCount.GetValueOrDefault(),
                KilledBy = s.KilledBy.GetValueOrDefault(),
                Kills = s.Kills.GetValueOrDefault(),
                PlayTime = s.PlayTime.GetValueOrDefault(),
                Score = s.Score.GetValueOrDefault()
            }).ToList();

            var profileStatsByFaction = character.StatsByFaction?.Select(s => new CharacterDetailsProfileStatByFaction
            {
                ProfileId = s.ProfileId,
                ProfileName = s.Profile?.Name,
                ImageId = s.Profile?.ImageId,
                Kills = new CharacterDetailsProfileStatByFactionValue
                {
                    Vs = s.KillsVS.GetValueOrDefault(),
                    Nc = s.KillsNC.GetValueOrDefault(),
                    Tr = s.KillsTR.GetValueOrDefault()
                },
                KilledBy = new CharacterDetailsProfileStatByFactionValue
                {
                    Vs = s.KilledByVS.GetValueOrDefault(),
                    Nc = s.KilledByNC.GetValueOrDefault(),
                    Tr = s.KilledByTR.GetValueOrDefault()
                }
            }).ToList();

            var weaponStats = character.WeaponStats?.Where(a => a.ItemId != 0).Select(s =>
            {
                double? accuracy = null;
                double? headshotRatio = null;
                double? killDeathRatio = null;
                double? killsPerHour = null;
                double? landedPerKill = null;
                double? shotsPerKill = null;
                double? scorePerMinute = null;
                double? vehicleKillsPerHour = null;

                double? kdrDelta = null;
                double? accuDelta = null;
                double? hsrDelta = null;
                double? kphDelta = null;
                double? vkphDelta = null;

                if (s.FireCount.Value > 0)
                {
                    accuracy = (double)s.HitCount.Value / s.FireCount.Value;
                }

                if (s.Kills.Value > 0)
                {
                    headshotRatio = (double)s.Headshots.Value / s.Kills.Value;
                    landedPerKill = (double)s.HitCount.Value / s.Kills.Value;
                    shotsPerKill = (double)s.FireCount.Value / s.Kills.Value;
                }

                if (s.Deaths.Value > 0)
                {
                    killDeathRatio = (double)s.Kills.Value / s.Deaths.Value;
                }

                if (s.PlayTime.Value > 0)
                {
                    killsPerHour = s.Kills.Value / (s.PlayTime.Value / 3600.0);
                    scorePerMinute = s.Score.Value / (s.PlayTime.Value / 60.0);
                    vehicleKillsPerHour = s.VehicleKills.Value / (s.PlayTime.Value / 3600.0);
                }

                if (aggregates != null && aggregates.TryGetValue($"{s.ItemId}-{s.VehicleId}", out var agg))
                {
                    if (killDeathRatio.HasValue && agg.STDKdr > 0)
                    {
                        kdrDelta = (killDeathRatio - agg.AVGKdr) / agg.STDKdr;
                    }

                    if (accuracy.HasValue && agg.STDAccuracy > 0)
                    {
                        accuDelta = (accuracy - agg.AVGAccuracy) / agg.STDAccuracy;
                    }

                    if (headshotRatio.HasValue && agg.STDHsr > 0)
                    {
                        hsrDelta = (headshotRatio - agg.AVGHsr) / agg.STDHsr;
                    }

                    if (killsPerHour.HasValue && agg.STDKph > 0)
                    {
                        kphDelta = (killsPerHour - agg.AVGKph) / agg.STDKph;
                    }

                    if (vehicleKillsPerHour.HasValue && agg.STDVkph > 0)
                    {
                        vkphDelta = (vehicleKillsPerHour - agg.AVGVkph) / agg.STDVkph;
                    }
                }

                return new CharacterDetailsWeaponStat
                {
                    ItemId = s.ItemId,
                    Name = s.Item?.Name,
                    Category = s.Item?.ItemCategory?.Name,
                    ImageId = s.Item?.ImageId,
                    VehicleId = s.VehicleId,
                    VehicleName = s.Vehicle?.Name,
                    VehicleImageId = s.Vehicle?.ImageId,
                    Stats = new CharacterDetailsWeaponStatValue
                    {
                        DamageGiven = s.DamageGiven,
                        DamageTakenBy = s.DamageTakenBy,
                        Kills = s.Kills,
                        Deaths = s.Deaths,
                        FireCount = s.FireCount,
                        HitCount = s.HitCount,
                        Headshots = s.Headshots,
                        KilledBy = s.KilledBy,
                        PlayTime = s.PlayTime,
                        Score = s.Score,
                        VehicleKills = s.VehicleKills,

                        Accuracy = accuracy,
                        HeadshotRatio = headshotRatio,
                        KillDeathRatio = killDeathRatio,
                        KillsPerHour = killsPerHour,
                        LandedPerKill = landedPerKill,
                        ShotsPerKill = shotsPerKill,
                        ScorePerMinute = scorePerMinute,
                        VehicleKillsPerHour = vehicleKillsPerHour,

                        KillDeathRatioDelta = kdrDelta,
                        AccuracyDelta = accuDelta,
                        HsrDelta = hsrDelta,
                        KphDelta = kphDelta,
                        VehicleKphDelta = vkphDelta
                    }
                };
            }).ToList();

            var characterVehicleStats = character.WeaponStats?.Where(a => a.VehicleId != 0).GroupBy(a => a.VehicleId).Select(s =>
            {
                var vehicleWeaponStats = s.Where(a => a.ItemId != 0);
                var vehicleStats = s.FirstOrDefault(a => a.ItemId == 0);

                return new CharacterDetailsVehicleStat
                {
                    // Gunner stats
                    VehicleId = s.Key,
                    DamageGiven = vehicleWeaponStats.Sum(a => a.DamageGiven.GetValueOrDefault()),
                    DamageTakenBy = vehicleWeaponStats.Sum(a => a.DamageTakenBy.GetValueOrDefault()),
                    Deaths = vehicleWeaponStats.Sum(a => a.Deaths.GetValueOrDefault()),
                    FireCount = vehicleWeaponStats.Sum(a => a.FireCount.GetValueOrDefault()),
                    HitCount = vehicleWeaponStats.Sum(a => a.HitCount.GetValueOrDefault()),
                    VehicleKills = vehicleWeaponStats.Sum(a => a.VehicleKills.GetValueOrDefault()),
                    Headshots = vehicleWeaponStats.Sum(a => a.Headshots.GetValueOrDefault()),
                    KilledBy = vehicleWeaponStats.Sum(a => a.KilledBy.GetValueOrDefault()),
                    Kills = vehicleWeaponStats.Sum(a => a.Kills.GetValueOrDefault()),
                    PlayTime = vehicleWeaponStats.Sum(a => a.PlayTime.GetValueOrDefault()),
                    Score = vehicleWeaponStats.Sum(a => a.Score.GetValueOrDefault()),

                    // Pilot stats
                    PilotDamageGiven = vehicleStats?.DamageGiven.GetValueOrDefault(),
                    PilotDamageTakenBy = vehicleStats?.DamageTakenBy.GetValueOrDefault(),
                    PilotDeaths = vehicleStats?.Deaths.GetValueOrDefault(),
                    PilotFireCount = vehicleStats?.FireCount.GetValueOrDefault(),
                    PilotHitCount = vehicleStats?.HitCount.GetValueOrDefault(),
                    PilotVehicleKills = vehicleStats?.VehicleKills.GetValueOrDefault(),
                    PilotHeadshots = vehicleStats?.Headshots.GetValueOrDefault(),
                    PilotKilledBy = vehicleStats?.KilledBy.GetValueOrDefault(),
                    PilotKills = vehicleStats?.Kills.GetValueOrDefault(),
                    PilotPlayTime = vehicleStats?.PlayTime.GetValueOrDefault(),
                    PilotScore = vehicleStats?.Score.GetValueOrDefault()
                };
            }).ToList();

            var characterStatsHistory = character.StatsHistory?.Select(a => new CharacterDetailsStatsHistory
            {
                StatName = a.StatName,
                AllTime = a.AllTime,
                OneLifeMax = a.OneLifeMax,
                Day = JToken.Parse(a.Day).ToObject<IEnumerable<int>>(),
                Week = JToken.Parse(a.Week).ToObject<IEnumerable<int>>(),
                Month = JToken.Parse(a.Month).ToObject<IEnumerable<int>>()
            }).ToList();

            var details = new CharacterDetails
            {
                Id = character.Id,
                Name = character.Name,
                FactionId = character.FactionId,
                Faction = character.Faction?.Name,
                FactionImageId = character.Faction?.ImageId,
                BattleRank = character.BattleRank,
                BattleRankPercentToNext = character.BattleRankPercentToNext,
                CertsEarned = character.CertsEarned,
                Title = character.Title?.Name,
                WorldId = character.WorldId,
                PrestigeLevel = character.PrestigeLevel,
                World = character.World?.Name,
                Times = times,
                LifetimeStats = lifetimeStats,
                ProfileStats = profileStats,
                ProfileStatsByFaction = profileStatsByFaction,
                WeaponStats = weaponStats,
                VehicleStats = characterVehicleStats,
                InfantryStats = CalculateInfantryStats(weaponStats, sanctionedWeaponIds),
                StatsHistory = characterStatsHistory
            };

            if (lifetimeStats.Deaths > 0 && details.InfantryStats != null)
            {
                details.InfantryStats.KDRPadding = lifetimeStats.Kills / (double)lifetimeStats.Deaths - details.InfantryStats.KillDeathRatio.GetValueOrDefault();
            }

            character.WeaponStats?.Where(a => a.VehicleId != 0 && a.ItemId == 0).GroupBy(a => a.VehicleId).ToList().ForEach(item =>
            {
                var vehicleStats = item.FirstOrDefault();

                var stat = details.VehicleStats.FirstOrDefault(a => a.VehicleId == item.Key);
                if (stat != null)
                {
                    stat.PilotKills = vehicleStats.Kills.GetValueOrDefault();
                    stat.PilotPlayTime = vehicleStats.PlayTime.GetValueOrDefault();
                    stat.PilotVehicleKills = vehicleStats.VehicleKills.GetValueOrDefault();
                }
            });

            if (character.OutfitMembership != null)
            {
                details.Outfit = new CharacterDetailsOutfit
                {
                    Id = character.OutfitMembership.Outfit.Id,
                    Name = character.OutfitMembership.Outfit.Name,
                    Alias = character.OutfitMembership.Outfit.Alias,
                    CreatedDate = character.OutfitMembership.Outfit.CreatedDate,
                    MemberCount = character.OutfitMembership.Outfit.MemberCount,
                    MemberSinceDate = character.OutfitMembership.MemberSinceDate.GetValueOrDefault(),
                    Rank = character.OutfitMembership.Rank
                };
            }

            return Task.FromResult(details);
        }

        private static InfantryStats CalculateInfantryStats(IEnumerable<CharacterDetailsWeaponStat> weaponStats, IEnumerable<int> sanctionedWeaponIds)
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

        private static Task<SimpleCharacterDetails> MapToSimpleCharacterDetailsAsync(CharacterDetails character)
        {
            var mostPlayedWeapon = character.WeaponStats.OrderByDescending(a => a.Stats.Kills)
                    .FirstOrDefault();
            var mostPlayedProfile = character.ProfileStats.Where(a => a.ProfileId != 0)
                .OrderByDescending(a => a.PlayTime)
                .FirstOrDefault();
            var playTimeInMax = character.ProfileStats.FirstOrDefault(a => a.ProfileId == 7)?.PlayTime;

            var details = new SimpleCharacterDetails
            {
                Id = character.Id,
                Name = character.Name,
                World = character.World,
                LastSaved = character.Times?.LastSaveDate,
                FactionId = character.FactionId,
                FactionName = character.Faction,
                FactionImageId = character.FactionImageId,
                BattleRank = character.BattleRank,
                OutfitAlias = character.Outfit?.Alias,
                OutfitName = character.Outfit?.Name,
                Kills = character.LifetimeStats.Kills,
                Deaths = character.LifetimeStats.Deaths,
                Score = character.LifetimeStats.Score,
                PlayTime = character.LifetimeStats.PlayTime,
                TotalPlayTimeMinutes = character.Times.MinutesPlayed,
                IVIScore = character.InfantryStats?.IVIScore ?? 0,
                IVIKillDeathRatio = character.InfantryStats?.KillDeathRatio ?? 0,
                Prestige = character.PrestigeLevel,
                MostPlayedWeaponName = mostPlayedWeapon?.Name,
                MostPlayedWeaponId = mostPlayedWeapon?.ItemId,
                MostPlayedWeaponKills = mostPlayedWeapon?.Stats.Kills.GetValueOrDefault(),
                MostPlayedClassName = mostPlayedProfile?.ProfileName,
                MostPlayedClassId = mostPlayedProfile?.ProfileId,
                PlayTimeInMax = playTimeInMax.GetValueOrDefault()
            };

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

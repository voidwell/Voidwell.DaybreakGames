using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Models;
using System;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using Microsoft.Extensions.Logging;
using DaybreakGames.Census.Exceptions;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IPlayerSessionRepository _playerSessionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly CensusCharacter _censusCharacter;
        private readonly ICache _cache;
        private readonly IOutfitService _outfitService;
        private readonly IWeaponAggregateService _weaponAggregateService;
        private readonly IGradeService _gradeService;
        private readonly IWeaponService _weaponService;
        private readonly ILogger<CharacterService> _logger;

        private static readonly string _cacheKey = "ps2.character";
        private Func<string, string> getDetailsCacheKey = (characterId) => $"{_cacheKey}_details_{characterId}";
        private readonly TimeSpan _cacheCharacterExpiration = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _cacheCharacterNameExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _cacheCharacterDetailsExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _cacheCharacterSessionsExpiration = TimeSpan.FromMinutes(10);

        private readonly KeyedSemaphoreSlim _characterLock = new KeyedSemaphoreSlim();
        private readonly SemaphoreSlim _characterStatsLock = new SemaphoreSlim(10);

        public CharacterService(ICharacterRepository characterRepository, IPlayerSessionRepository playerSessionRepository,
            IEventRepository eventRepository, CensusCharacter censusCharacter, ICache cache, IOutfitService outfitService,
            IWeaponAggregateService weaponAggregateService, IGradeService gradeService, IWeaponService weaponService, ILogger<CharacterService> logger)
        {
            _characterRepository = characterRepository;
            _playerSessionRepository = playerSessionRepository;
            _eventRepository = eventRepository;
            _censusCharacter = censusCharacter;
            _cache = cache;
            _outfitService = outfitService;
            _weaponAggregateService = weaponAggregateService;
            _gradeService = gradeService;
            _weaponService = weaponService;
            _logger = logger;
        }

        public async Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12)
        {
            var characterList = await _censusCharacter.LookupCharactersByName(query, limit);
            return characterList.Select(c => CharacterSearchResult.LoadFromCensusCharacter(c));
        }

        public Task<IEnumerable<Character>> FindCharacters(IEnumerable<string> characterIds)
        {
            return _characterRepository.GetCharactersByIdsAsync(characterIds);
        }

        public async Task<Character> GetCharacter(string characterId)
        {
            Character character = null;

            using (await _characterLock.WaitAsync(characterId))
            {
                var cacheKey = $"{_cacheKey}_character_{characterId}";

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

        public async Task<CharacterDetails> GetCharacterDetails(string characterId)
        {
            var details = await GetCharacterDetailsFromCacheAsync(characterId);

            if (details != null)
            {
                return details;
            }

            var characterTask = GetCharacterDetailsAsync(characterId);
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

        private async Task<IEnumerable<CharacterDetails>> GetCharacterDetails(IEnumerable<string> characterIds)
        {
            var cachedDetails = (await Task.WhenAll(characterIds.Select(GetCharacterDetailsFromCacheAsync))).Where(a => a != null);

            var remainingCharacterIds = characterIds.Where(a => !cachedDetails.Any(b => b.Id == a));
            if (!remainingCharacterIds.Any())
            {
                return cachedDetails;
            }

            var details = new List<CharacterDetails>();
            details.AddRange(cachedDetails);

            var charactersTask = GetCharacterDetailsAsync(remainingCharacterIds);
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
            var cacheKey = getDetailsCacheKey(characterId);
            return _cache.GetAsync<CharacterDetails>(cacheKey);
        }

        private Task SaveCharacterDetailsInCacheAsync(CharacterDetails details)
        {
            var cacheKey = getDetailsCacheKey(details.Id);
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
                var vehicleStats = s.Where(a => a.ItemId == 0).FirstOrDefault();

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
                    PilotDamageGiven = vehicleStats?.DamageGiven.Value,
                    PilotDamageTakenBy = vehicleStats?.DamageTakenBy.Value,
                    PilotDeaths = vehicleStats?.Deaths.Value,
                    PilotFireCount = vehicleStats?.FireCount.Value,
                    PilotHitCount = vehicleStats?.HitCount.Value,
                    PilotVehicleKills = vehicleStats?.VehicleKills.Value,
                    PilotHeadshots = vehicleStats?.Headshots.Value,
                    PilotKilledBy = vehicleStats?.KilledBy.Value,
                    PilotKills = vehicleStats?.Kills.Value,
                    PilotPlayTime = vehicleStats?.PlayTime.Value,
                    PilotScore = vehicleStats?.Score.Value
                };
            }).ToList();

            var characterStatsHistory = character.StatsHistory?.Select(a =>
            {
                return new CharacterDetailsStatsHistory
                {
                    StatName = a.StatName,
                    AllTime = a.AllTime,
                    OneLifeMax = a.OneLifeMax,
                    Day = JToken.Parse(a.Day).ToObject<IEnumerable<int>>(),
                    Week = JToken.Parse(a.Week).ToObject<IEnumerable<int>>(),
                    Month = JToken.Parse(a.Month).ToObject<IEnumerable<int>>(),
                };
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
                details.InfantryStats.KDRPadding = (lifetimeStats.Kills / (double)lifetimeStats.Deaths) - details.InfantryStats.KillDeathRatio.GetValueOrDefault();
            }

            character.WeaponStats?.Where(a => a.VehicleId != 0 && a.ItemId == 0).GroupBy(a => a.VehicleId).ToList().ForEach(item =>
            {
                var vehicleStats = item.FirstOrDefault();

                var stat = details.VehicleStats.Where(a => a.VehicleId == item.Key).FirstOrDefault();
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
            var sumDeaths = sanctionedWeapons.Sum(a => a.Stats.Deaths);
            var sumHeadshots = sanctionedWeapons.Sum(a => a.Stats.Headshots);
            var sumPlayTime = sanctionedWeapons.Sum(a => a.Stats.PlayTime);

            var sumUnsanctionedKills = unSanctionedWeapons.Sum(a => a.Stats.Kills);
            var sumUnsanctionedDeaths = unSanctionedWeapons.Sum(a => a.Stats.Deaths);

            var infantryStats = new InfantryStats
            {
                Weapons = sanctionedWeapons.Count(),
                UnsanctionedWeapons = unSanctionedWeapons.Count(),
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

            infantryStats.IVIScore = (int)Math.Round((infantryStats.HeadshotRatio.GetValueOrDefault() * 100) * (infantryStats.Accuracy.GetValueOrDefault() * 100), 0);

            return infantryStats;
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
                               UpdateCharacterStatsHistory(characterId, lastLoginDate));

            var characterCacheKey = $"{_cacheKey}_character_{characterId}";
            var detailsCacheKey = $"{_cacheKey}_details_{characterId}";
            await Task.WhenAll(_cache.RemoveAsync(characterCacheKey), _cache.RemoveAsync(detailsCacheKey));
        }

        public async Task<OutfitMember> GetCharactersOutfit(string characterId)
        {
            var character = await GetCharacter(characterId);
            if (character == null)
            {
                return null;
            }

            return await _outfitService.UpdateCharacterOutfitMembership(character);
        }

        public async Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetSessions(string characterId)
        {
            var cacheKey = $"{_cacheKey}_sessions_{characterId}";

            var sessions = await _cache.GetAsync<IEnumerable<Data.Models.Planetside.PlayerSession>>(cacheKey);
            if (sessions != null)
            {
                return sessions;
            }

            sessions = await _playerSessionRepository.GetPlayerSessionsByCharacterIdAsync(characterId, 25);

            await _cache.SetAsync(cacheKey, sessions, _cacheCharacterSessionsExpiration);

            return sessions;
        }

        public async Task<Models.PlayerSession> GetSession(string characterId, int sessionId)
        {
            var cacheKey = $"{_cacheKey}_session_{characterId}_{sessionId}";

            var sessionInfo = await _cache.GetAsync<Models.PlayerSession>(cacheKey);
            if (sessionInfo != null)
            {
                return sessionInfo;
            }

            var playerSession = await _playerSessionRepository.GetPlayerSessionAsync(sessionId);
            if (playerSession == null)
            {
                return null;
            }

            var sessionEvents = await GetSessionEventsForCharacter(characterId, playerSession.LoginDate, playerSession.LogoutDate);

            sessionEvents.Insert(0, new PlayerSessionLoginEvent { Timestamp = playerSession.LoginDate });
            sessionEvents.Add(new PlayerSessionLogoutEvent { Timestamp = playerSession.LogoutDate });

            sessionInfo = new Models.PlayerSession
            {
                Events = sessionEvents,
                Session = new PlayerSessionInfo
                {
                    CharacterId = playerSession.CharacterId,
                    Id = playerSession.Id.ToString(),
                    Duration = playerSession.Duration,
                    LoginDate = playerSession.LoginDate,
                    LogoutDate = playerSession.LogoutDate
                }
            };

            await _cache.SetAsync(cacheKey, sessionInfo, _cacheCharacterSessionsExpiration);

            return sessionInfo;
        }

        private async Task<List<PlayerSessionEvent>> GetSessionEventsForCharacter(string characterId, DateTime start, DateTime end)
        {
            var sessionDeathsTask = _eventRepository.GetDeathEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionFacilityCapturesTask = _eventRepository.GetFacilityCaptureEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionFacilityDefendsTask = _eventRepository.GetFacilityDefendEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionBattleRankUpsTask = _eventRepository.GetBattleRankUpEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionVehicleDestroysTask = _eventRepository.GetVehicleDestroyEventsForCharacterIdByDateAsync(characterId, start, end);

            await Task.WhenAll(sessionDeathsTask, sessionFacilityCapturesTask, sessionFacilityDefendsTask, sessionBattleRankUpsTask, sessionVehicleDestroysTask);

            var sessionDeaths = sessionDeathsTask.Result;
            var sessionFacilityCaptures = sessionFacilityCapturesTask.Result;
            var sessionFacilityDefends = sessionFacilityDefendsTask.Result;
            var sessionBattleRankUps = sessionBattleRankUpsTask.Result;
            var sessionVehicleDestroys = sessionVehicleDestroysTask.Result;

            var sessionEvents = new List<PlayerSessionEvent>();

            sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(sessionDeaths));
            sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(sessionFacilityCaptures));
            sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(sessionFacilityDefends));
            sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(sessionBattleRankUps));
            sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(sessionVehicleDestroys));

            return sessionEvents.OrderBy(a => a.Timestamp).ToList();
        }

        private async Task<Character> GetCharacterDetailsAsync(string characterId)
        {
            var character = await _characterRepository.GetCharacterWithDetailsAsync(characterId);
            if (character != null && character.Time != null)
            {
                return character;
            }

            await UpdateAllCharacterInfo(characterId, null);

            return await _characterRepository.GetCharacterWithDetailsAsync(characterId);
        }

        private async Task<IEnumerable<Character>> GetCharacterDetailsAsync(IEnumerable<string> characterIds)
        {
            var characters = await _characterRepository.GetCharacterWithDetailsAsync(characterIds.ToArray());

            var completeDetails = characters.Where(a => a.Time != null).ToList();

            var missingCharacterIds = characterIds.Where(a => !characters.Any(b => b.Id == a)).ToList();

            if (missingCharacterIds.Any())
            {
                var updateTasks = missingCharacterIds.Select(a => UpdateAllCharacterInfo(a, null));

                await Task.WhenAll(updateTasks);

                var updatedCharacters = await _characterRepository.GetCharacterWithDetailsAsync(missingCharacterIds.ToArray());

                completeDetails.AddRange(updatedCharacters);
            }

            return completeDetails;
        }

        private async Task<Character> UpdateCharacter(string characterId)
        {
            var character = await _censusCharacter.GetCharacter(characterId);

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

            await _outfitService.UpdateCharacterOutfitMembership(model);

            return model;
        }

        private async Task UpdateCharacterTimes(string characterId)
        {
            var times = await _censusCharacter.GetCharacterTimes(characterId);

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
            var statsTask = _censusCharacter.GetCharacterStats(characterId, LastLoginDate);
            var statsByFactionTask = _censusCharacter.GetCharacterFactionStats(characterId, LastLoginDate);

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
            var characterWepStatsTask = _censusCharacter.GetCharacterWeaponStats(characterId, LastLoginDate);
            var characterWepStatsByFactionTask = _censusCharacter.GetCharacterWeaponStatsByFaction(characterId, LastLoginDate);

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
            var statsHistory = await _censusCharacter.GetCharacterStatsHistory(characterId, lastLoginDate);
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

        private async Task<string> GetCharacterIdByName(string characterName)
        {
            var cacheKey = $"{_cacheKey}_name_{characterName.ToLower()}";

            var characterId = await _cache.GetAsync<string>(cacheKey);
            if (characterId != null)
            {
                return characterId;
            }

            characterId = await _characterRepository.GetCharacterIdByName(characterName);
            if (characterId == null)
            {
                characterId = await _censusCharacter.GetCharacterIdByName(characterName);
            }

            if (characterId != null)
            {
                await _cache.SetAsync(cacheKey, characterId, _cacheCharacterNameExpiration);
            }

            return characterId;
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
                IVIScore = character.InfantryStats != null ? character.InfantryStats.IVIScore.GetValueOrDefault() : 0,
                IVIKillDeathRatio = character.InfantryStats != null ? character.InfantryStats.KillDeathRatio.GetValueOrDefault() : 0,
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
            var characterIdResults = await Task.WhenAll(characterNames.Select(GetCharacterIdByName));
            var characterIds = characterIdResults.Where(a => !string.IsNullOrWhiteSpace(a));

            if (!characterIds.Any())
            {
                return null;
            }

            return await GetCharacterDetails(characterIds);
        }

        private async Task<CharacterDetails> GetCharacterDetailsByNameAsync(string characterName)
        {
            var characterId = await GetCharacterIdByName(characterName);
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

            if (int.TryParse(weaponSearch, out int weaponId))
            {
                weaponStats = validStats.FirstOrDefault(a => a.ItemId == weaponId);
            }

            if (weaponStats == null)
            {
                weaponStats = validStats
                    .OrderByDescending(a => a.Stats.Kills)
                    .ThenBy(a => a.Name)
                    .FirstOrDefault(a => a.Name.ToLower().Contains(weaponSearch.ToLower()));
            }

            return weaponStats;
        }
    }
}

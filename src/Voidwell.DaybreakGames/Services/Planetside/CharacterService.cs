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
using Voidwell.DaybreakGames.Census.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private readonly ILogger<CharacterService> _logger;

        private readonly string _cacheKey = "ps2.character";
        private readonly TimeSpan _cacheCharacterExpiration = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _cacheCharacterDetailsExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _cacheCharacterSessionsExpiration = TimeSpan.FromMinutes(10);

        public CharacterService(ICharacterRepository characterRepository, IPlayerSessionRepository playerSessionRepository,
            IEventRepository eventRepository, CensusCharacter censusCharacter, ICache cache, IOutfitService outfitService,
            ILogger<CharacterService> logger)
        {
            _characterRepository = characterRepository;
            _playerSessionRepository = playerSessionRepository;
            _eventRepository = eventRepository;
            _censusCharacter = censusCharacter;
            _cache = cache;
            _outfitService = outfitService;
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

        private readonly KeyedSemaphoreSlim _characterLock = new KeyedSemaphoreSlim();

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
            var cacheKey = $"{_cacheKey}_details_{characterId}";

            var details = await _cache.GetAsync<CharacterDetails>(cacheKey);
            if (details != null)
            {
                return details;
            }

            var character = await GetCharacterDetailsAsync(characterId);
            if (character == null)
            {
                return null;
            }

            details = new CharacterDetails
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
                World = character.World?.Name,
                Times = new CharacterDetailsTimes
                {
                    CreatedDate = character.Time.CreatedDate,
                    LastLoginDate = character.Time.LastLoginDate,
                    LastSaveDate = character.Time.LastSaveDate,
                    MinutesPlayed = character.Time.MinutesPlayed
                },
                LifetimeStats = new CharacterDetailsLifetimeStats
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
                },
                ProfileStats = character.Stats?.Select(s => new CharacterDetailsProfileStat
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
                }),
                ProfileStatsByFaction = character.StatsByFaction?.Select(s => new CharacterDetailsProfileStatByFaction
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
                }),
                WeaponStats = character.WeaponStats?.Where(a => a.ItemId != 0).Select(s => new CharacterDetailsWeaponStat
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
                        /*
                        Accuracy = s.HitCount.Value > 0 ? s.FireCount.Value / s.HitCount.Value : 0,
                        HeadshotRatio = s.Kills.Value > 0 ? (s.Headshots.Value / s.Kills.Value) * 100 : 0,
                        KillDeathRatio = s.Deaths.Value > 0 ? s.Kills.Value / s.Deaths.Value : 0,
                        KillsPerHour = s.PlayTime.Value > 0 ? s.Kills.Value / (s.PlayTime.Value / 3600) : 0,
                        LandedPerKill = s.Kills.Value > 0 ? s.HitCount.Value / s.Kills.Value : 0,
                        ShotsPerKill = s.Kills.Value > 0 ? s.FireCount.Value / s.Kills.Value : 0,
                        ScorePerMinute = s.PlayTime.Value > 0 ? s.Score.Value / (s.PlayTime.Value / 60) : 0,
                        VehicleKillsPerHour = s.Kills.Value > 0 ? s.VehicleKills.Value / (s.Kills.Value / 3600) : 0
                        */
                    }
                }),
                VehicleStats = character.WeaponStats?.Where(a => a.VehicleId != 0).GroupBy(a => a.VehicleId).Select(s =>
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
                })
            };

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

            await _cache.SetAsync(cacheKey, details, _cacheCharacterDetailsExpiration);

            return details;
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
                               UpdateCharacterWeaponStats(characterId, lastLoginDate));
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

            var sessionDeaths = await _eventRepository.GetDeathEventsForCharacterIdByDateAsync(characterId, playerSession.LoginDate, playerSession.LogoutDate);

            var sessionEvents = sessionDeaths.Select(d => new PlayerSessionEvent
            {
                Attacker = new CombatReportItemDetail { Id = d.AttackerCharacterId, Name = d.AttackerCharacter?.Name ?? d.AttackerCharacterId, FactionId = d.AttackerCharacter?.FactionId },
                Victim = new CombatReportItemDetail { Id = d.CharacterId, Name = d.Character?.Name ?? d.CharacterId, FactionId = d.Character?.FactionId },
                Weapon = new PlayerSessionWeapon { Id = d.AttackerWeaponId.Value, ImageId = d.AttackerWeapon?.ImageId, Name = d.AttackerWeapon?.Name ?? d.AttackerWeaponId.Value.ToString() },
                Timestamp = d.Timestamp,
                ZoneId = d.ZoneId,
                IsHeadshot = d.IsHeadshot,
                AttackerFireModeId = d.AttackerFireModeId,
                AttackerLoadoutId = d.AttackerLoadoutId,
                AttackerOutfitId = d.AttackerOutfitId,
                AttackerVehicleId = d.AttackerVehicleId,
                CharacterLoadoutId = d.CharacterLoadoutId,
                CharacterOutfitId = d.CharacterOutfitId
            });

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

        public async Task<Character> GetCharacterDetailsAsync(string characterId)
        {
            var character = await _characterRepository.GetCharacterWithDetailsAsync(characterId);
            if (character != null && character.Time != null)
            {
                return character;
            }

            await UpdateAllCharacterInfo(characterId, null);

            return await _characterRepository.GetCharacterWithDetailsAsync(characterId);
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
                TitleId = character.TitleId
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

            var statGroups = stats.GroupBy(a => a.ProfileId);
            var statByFactionGroups = statsByFaction.GroupBy(a => a.ProfileId);

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

            var statGroups = wepStats.GroupBy(a => new { a.ItemId, a.VehicleId });
            var statByFactionGroups = wepStatsByFaction.GroupBy(a => new { a.ItemId, a.VehicleId });

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
    }
}

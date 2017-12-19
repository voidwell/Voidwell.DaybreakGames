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
        public readonly ILogger<CharacterService> _logger;

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

        public Task<IEnumerable<DbCharacter>> FindCharacters(IEnumerable<string> characterIds)
        {
            return _characterRepository.GetCharactersByIdsAsync(characterIds);
        }

        public async Task<DbCharacter> GetCharacter(string characterId)
        {
            var character = await _characterRepository.GetCharacterAsync(characterId);
            return character ?? await UpdateCharacter(characterId);
        }

        public async Task<CharacterDetails> GetCharacterDetails(string characterId)
        {
            var character = await GetCharacterDetailsAsync(characterId);

            var lifetimeStats = character.Stats.FirstOrDefault(a => a.ProfileId == "0");

            var details = new CharacterDetails
            {
                Id = character.Id,
                Name = character.Name,
                FactionId = character.FactionId,
                Faction = character.Faction.Name,
                FactionImageId = character.Faction.ImageId,
                BattleRank = character.BattleRank,
                BattleRankPercentToNext = character.BattleRankPercentToNext,
                CertsEarned = character.CertsEarned,
                Title = character.Title.Name,
                WorldId = character.WorldId,
                World = character.World.Name,
                Outfit = new CharacterDetailsOutfit
                {
                    Id = character.OutfitMembership.Outfit.Id,
                    Name = character.OutfitMembership.Outfit.Name,
                    Alias = character.OutfitMembership.Outfit.Alias,
                    CreatedDate = character.OutfitMembership.Outfit.CreatedDate,
                    MemberCount = character.OutfitMembership.Outfit.MemberCount,
                    MemberSinceDate = character.OutfitMembership.MemberSinceDate,
                    Rank = character.OutfitMembership.Rank
                },
                Times = new CharacterDetailsTimes
                {
                    CreatedDate = character.Time.CreatedDate,
                    LastLoginDate = character.Time.LastLoginDate,
                    LastSaveDate = character.Time.LastSaveDate,
                    MinutesPlayed = character.Time.MinutesPlayed
                },
                LifetimeStats = new CharacterDetailsLifetimeStats
                {
                    AchievementCount = lifetimeStats.AchievementCount.Value,
                    AssistCount = lifetimeStats.AssistCount.Value,
                    DamageGiven = lifetimeStats.WeaponDamageGiven.Value,
                    DamageTakenBy = lifetimeStats.WeaponDamageTakenBy.Value,
                    Deaths = lifetimeStats.Deaths.Value,
                    DominationCount = lifetimeStats.DominationCount.Value,
                    FacilityCaptureCount = lifetimeStats.FacilityCaptureCount.Value,
                    FacilityDefendedCount = lifetimeStats.FacilityDefendedCount.Value,
                    FireCount = lifetimeStats.FireCount.Value,
                    HitCount = lifetimeStats.HitCount.Value,
                    Headshots = lifetimeStats.WeaponHeadshots.Value,
                    KilledBy = lifetimeStats.KilledBy.Value,
                    Kills = lifetimeStats.Kills.Value,
                    MedalCount = lifetimeStats.MedalCount.Value,
                    PlayTime = lifetimeStats.PlayTime.Value,
                    RevengeCount = lifetimeStats.RevengeCount.Value,
                    Score = lifetimeStats.Score.Value,
                    SkillPoints = lifetimeStats.Kills.Value,
                    VehicleKills = lifetimeStats.WeaponVehicleKills.Value
                },
                ProfileStats = character.Stats.Select(s => new CharacterDetailsProfileStat
                {
                    ProfileId = s.ProfileId,
                    ProfileName = s.Profile?.Name,
                    ImageId = s.Profile?.ImageId,
                    Deaths = s.Deaths.Value,
                    FireCount = s.FireCount.Value,
                    HitCount = s.HitCount.Value,
                    KilledBy = s.KilledBy.Value,
                    Kills = s.Kills.Value,
                    PlayTime = s.PlayTime.Value,
                    Score = s.Score.Value
                }),
                ProfileStatsByFaction = character.StatsByFaction.Select(s => new CharacterDetailsProfileStatByFaction
                {
                    ProfileId = s.ProfileId,
                    ProfileName = s.Profile?.Name,
                    ImageId = s.Profile?.ImageId,
                    Kills = new CharacterDetailsProfileStatByFactionValue
                    {
                        Vs = s.KillsVS.Value,
                        Nc = s.KillsNC.Value,
                        Tr = s.KillsTR.Value
                    },
                    KilledBy = new CharacterDetailsProfileStatByFactionValue
                    {
                        Vs = s.KilledByVS.Value,
                        Nc = s.KilledByNC.Value,
                        Tr = s.KilledByTR.Value
                    }
                }),
                WeaponStats = character.WeaponStats.Select(s => new CharacterDetailsWeaponStat
                {
                    ItemId = s.ItemId,
                    Name = s.Item?.Name,
                    Category = s.Item?.ItemCategoryId,
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
                VehicleStats = character.WeaponStats.GroupBy(a => a.VehicleId).Where(a => a.Key != null || a.Key != "0").Select(s => new CharacterDetailsVehicleStat
                {
                    VehicleId = s.Key,
                    DamageGiven = s.Sum(a => a.DamageGiven.Value),
                    DamageTakenBy = s.Sum(a => a.DamageTakenBy.Value),
                    Deaths = s.Sum(a => a.Deaths.Value),
                    FireCount = s.Sum(a => a.FireCount.Value),
                    VehicleKills = s.Sum(a => a.VehicleKills.Value),
                    Headshots = s.Sum(a => a.Headshots.Value),
                    HitCount = s.Sum(a => a.HitCount.Value),
                    KilledBy = s.Sum(a => a.KilledBy.Value),
                    Kills = s.Sum(a => a.Kills.Value),
                    PlayTime = s.Sum(a => a.PlayTime.Value),
                    Score = s.Sum(a => a.Score.Value)
                })
            };

            return details;
        }

        public async Task UpdateCharacter(string characterId, DateTime? lastLoginDate = null)
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

        public Task<DbOutfitMember> GetCharactersOutfit(string characterId)
        {
            return _outfitService.UpdateCharacterOutfitMembership(characterId);
        }

        public Task<IEnumerable<DbPlayerSession>> GetSessions(string characterId)
        {
            return _playerSessionRepository.GetPlayerSessionsByCharacterIdAsync(characterId, 25);
        }

        public async Task<PlayerSession> GetSession(string characterId, string sessionId)
        {
            var session = await _playerSessionRepository.GetPlayerSessionAsync(sessionId);
            if (session == null)
            {
                return null;
            }

            var sessionDeaths = await _eventRepository.GetDeathEventsForCharacterIdByDateAsync(characterId, session.LoginDate, session.LogoutDate);

            var sessionEvents = sessionDeaths.Select(d => new PlayerSessionEvent
            {
                Attacker = new CombatReportItemDetail { Id = d.AttackerCharacterId, Name = d.AttackerCharacter.Name, FactionId = d.AttackerCharacter.FactionId },
                Victim = new CombatReportItemDetail { Id = d.CharacterId, Name = d.Character.Name, FactionId = d.Character.FactionId },
                Weapon = new PlayerSessionWeapon { Id = d.AttackerWeaponId, ImageId = d.AttackerWeapon.ImageId, Name = d.AttackerWeapon.Name },
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

            return new PlayerSession
            {
                Events = sessionEvents,
                Session = new PlayerSessionInfo
                {
                    CharacterId = session.CharacterId,
                    Id = session.Id,
                    Duration = session.Duration,
                    LoginDate = session.LoginDate,
                    LogoutDate = session.LogoutDate
                }
            };
        }

        public async Task<DbCharacter> GetCharacterDetailsAsync(string characterId)
        {
            var character = await _characterRepository.GetCharacterWithDetailsAsync(characterId);
            if (character != null && character.Time != null)
            {
                return character;
            }

            await UpdateCharacter(characterId, null);

            return await _characterRepository.GetCharacterWithDetailsAsync(characterId);
        }

        private async Task<DbCharacter> UpdateCharacter(string characterId)
        {
            var character = await _censusCharacter.GetCharacter(characterId);

            if (character == null)
            {
                return null;
            }

            var dataModel = new DbCharacter
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

            await _characterRepository.UpsertAsync(dataModel);

            await _outfitService.UpdateCharacterOutfitMembership(characterId);

            return dataModel;
        }

        private async Task UpdateCharacterTimes(string characterId)
        {
            var times = await _censusCharacter.GetCharacterTimes(characterId);

            if (times == null)
            {
                return;
            }

            var dataModel = new DbCharacterTime
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

            var statModels = new List<DbCharacterStat>();
            var statByFactionModels = new List<DbCharacterStatByFaction>();

            var statGroups = stats.GroupBy(a => a.ProfileId);
            var statByFactionGroups = statsByFaction.GroupBy(a => a.ProfileId);

            foreach (var group in statGroups)
            {
                var dbModel = new DbCharacterStat
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
                var dbModel = new DbCharacterStatByFaction
                {
                    CharacterId = characterId,
                    ProfileId = group.Key
                };

                var statModel = statModels.SingleOrDefault(a => a.ProfileId == group.Key);
                if (statModel == null)
                {
                    statModel = new DbCharacterStat
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

            var statModels = new List<DbCharacterWeaponStat>();
            var statByFactionModels = new List<DbCharacterWeaponStatByFaction>();

            var statGroups = wepStats.GroupBy(a => a.ItemId);
            var statByFactionGroups = wepStatsByFaction.GroupBy(a => a.ItemId);

            foreach (var group in statGroups)
            {
                var dbModel = new DbCharacterWeaponStat
                {
                    CharacterId = characterId,
                    ItemId = group.Key,
                    VehicleId = group.First().VehicleId
                };

                foreach (var stat in group)
                {
                    StatValueMapper.AssignStatValue(ref dbModel, stat.StatName, stat.Value);
                }

                statModels.Add(dbModel);
            }

            foreach (var group in statByFactionGroups)
            {
                var dbModel = new DbCharacterWeaponStatByFaction
                {
                    CharacterId = characterId,
                    ItemId = group.Key,
                    VehicleId = group.First().VehicleId
                };

                var statModel = statModels.SingleOrDefault(a => a.ItemId == group.Key);
                if (statModel == null)
                {
                    statModel = new DbCharacterWeaponStat
                    {
                        CharacterId = characterId,
                        ItemId = group.Key,
                        VehicleId = group.First().VehicleId
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

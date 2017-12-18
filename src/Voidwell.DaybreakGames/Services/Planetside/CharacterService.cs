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
using Voidwell.DaybreakGames.CensusServices.Models;

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

        public async Task<DbCharacter> GetCharacterDetails(string characterId)
        {
            var character = await TryGetCharacterFull(characterId);

            if (character != null && character.Time != null)
            {
                return character;
            }

            await UpdateCharacter(characterId, null);

            return await TryGetCharacterFull(characterId);
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

        private Task<DbCharacter> TryGetCharacterFull(string characterId)
        {
            return _characterRepository.GetCharacterWithDetailsAsync(characterId);
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

                foreach(var stat in group)
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
    }
}

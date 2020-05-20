using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class PlayerMonitor : IPlayerMonitor
    {
        private readonly ICharacterService _characterService;
        private readonly ICharacterUpdaterService _updaterService;
        private readonly IPlayerSessionRepository _playerSessionRepository;
        private readonly ICharacterRatingService _characterRatingService;
        private readonly ICache _cache;

        private static readonly Func<int, string> GetListCacheKey = worldId => $"ps2.online-players_world_{worldId}";
        private static readonly Func<string, string> GetPlayerCacheKey = characterId => $"ps2.online-players_character_{characterId}";
        private static readonly TimeSpan CacheIdleDuration = TimeSpan.FromHours(3);
        private static readonly TimeSpan MaximumIdleDuration = TimeSpan.FromMinutes(10);

        public PlayerMonitor(ICharacterService characterService, ICharacterUpdaterService updaterService,
            IPlayerSessionRepository playerSessionRepository, ICharacterRatingService characterRatingService,
            ICache cache)
        {
            _characterService = characterService;
            _updaterService = updaterService;
            _playerSessionRepository = playerSessionRepository;
            _characterRatingService = characterRatingService;
            _cache = cache;
        }

        public async Task<OnlineCharacter> SetOnlineAsync(string characterId, DateTime timestamp)
        {
            var character = await _characterService.GetCharacter(characterId);
            if (character == null)
            {
                return null;
            }

            var onlineCharacter = new OnlineCharacter
            {
                Character = new OnlineCharacterProfile
                {
                    CharacterId = character.Id,
                    FactionId = character.FactionId,
                    Name = character.Name,
                    WorldId = character.WorldId
                },
                LoginDate = timestamp
            };

            await Task.WhenAll(
                _cache.AddToListAsync(GetListCacheKey(character.WorldId), character.Id),
                _cache.SetAsync(GetPlayerCacheKey(character.Id), onlineCharacter, CacheIdleDuration));

            return onlineCharacter;
        }

        public async Task<OnlineCharacter> SetOfflineAsync(string characterId, DateTime timestamp)
        {
            var character = await _characterService.GetCharacter(characterId);
            if (character == null)
            {
                return null;
            }

            var onlineCharacter = await _cache.GetAsync<OnlineCharacter>(GetPlayerCacheKey(characterId));
            if (onlineCharacter == null)
            {
                return null;
            }

            var duration = timestamp - onlineCharacter.LoginDate;
            if (duration.TotalMinutes >= 5)
            {
                await _updaterService.AddToQueue(characterId);
            }

            var dataModel = new Data.Models.Planetside.PlayerSession
            {
                CharacterId = characterId,
                LoginDate = onlineCharacter.LoginDate,
                LogoutDate = timestamp,
                Duration = (int)duration.TotalMilliseconds
            };

            await Task.WhenAll(
                _playerSessionRepository.AddAsync(dataModel),
                RemoveFromCacheList(character),
                _characterRatingService.SaveCachedRatingAsync(characterId));

            return onlineCharacter;
        }

        public async Task<OnlineCharacter> SetLastSeenAsync(string characterId, int zoneId, DateTime timestamp)
        {
            var onlineCharacter = await GetAsync(characterId);
            if (onlineCharacter == null)
            {
                onlineCharacter = await SetOnlineAsync(characterId, timestamp);
                if (onlineCharacter == null)
                {
                    return null;
                }
            }

            onlineCharacter.UpdateLastSeen(timestamp, zoneId);
            await _cache.SetAsync(GetPlayerCacheKey(characterId), onlineCharacter, CacheIdleDuration);

            return onlineCharacter;
        }

        public async Task<IEnumerable<OnlineCharacter>> GetAllAsync(int worldId, int? zoneId = null)
        {
            var idList = await _cache.GetListAsync(GetListCacheKey(worldId));

            var taskList = idList.Select(characterId => _cache.GetAsync<OnlineCharacter>(GetPlayerCacheKey(characterId))).ToList();

            var characters = (await Task.WhenAll(taskList))?.Where(a => a != null);

            var timedOutCharacters = idList.Where(characterId => !characters.Any(a => a.Character.CharacterId == characterId)).ToList();
            if (timedOutCharacters.Any())
            {
                var clearTimedOutTasks = timedOutCharacters.Select(characterId => _cache.RemoveFromListAsync(GetListCacheKey(worldId), characterId));
                await Task.WhenAll(clearTimedOutTasks);
            }

            if (zoneId == null)
            {
                return characters;
            }

            return characters.Where(a => a.LastSeen?.ZoneId == zoneId && DateTime.UtcNow - a.LastSeen.Timestamp <= MaximumIdleDuration);
        }

        public Task<long> GetPlayerCountAsync(int worldId)
        {
            return _cache.GetListLengthAsync(GetListCacheKey(worldId));
        }

        public Task<OnlineCharacter> GetAsync(string characterId)
        {
            return _cache.GetAsync<OnlineCharacter>(GetPlayerCacheKey(characterId));
        }

        public Task ClearWorldAsync(int worldId)
        {
            return _cache.RemoveAsync(GetListCacheKey(worldId));
        }

        private Task RemoveFromCacheList(Character character)
        {
            return Task.WhenAll(_cache.RemoveFromListAsync(GetListCacheKey(character.WorldId), character.Id),
                _cache.RemoveAsync(GetPlayerCacheKey(character.Id)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames.GameState
{
    public class PlayerMonitor : IPlayerMonitor
    {
        private static readonly TimeSpan LastSeenLifetime = TimeSpan.FromMinutes(10);
        private static readonly Func<int, string> GetListCacheKey = worldId => $"ps2.player-monitor_world_{worldId}";
        private static readonly Func<string, string> GetPlayerCacheKey = characterId => $"ps2.player-monitor_character_{characterId}";
        private static readonly TimeSpan CacheIdleDuration = TimeSpan.FromHours(3);

        private readonly ICache _cache;

        public PlayerMonitor(ICache cache)
        {
            _cache = cache;
        }

        public Task ClearAllCharactersAsync(int worldId)
        {
            return _cache.RemoveAsync(GetListCacheKey(worldId));
        }

        public async Task<IEnumerable<OnlineCharacter>> GetOnlineCharactersAsync(int worldId, int? zoneId = null)
        {
            var idList = await _cache.GetListAsync(GetListCacheKey(worldId));

            var taskList = idList.Select(characterId => _cache.GetAsync<OnlineCharacter>(GetPlayerCacheKey(characterId))).ToList();

            var characters = (await Task.WhenAll(taskList))?.Where(a => a != null);

            var timedOutCharacters = idList.Where(characterId => !characters.Any(a => a.CharacterId == characterId)).ToList();
            if (timedOutCharacters.Any())
            {
                var clearTimedOutTasks = timedOutCharacters.Select(characterId => _cache.RemoveFromListAsync(GetListCacheKey(worldId), characterId));
                await Task.WhenAll(clearTimedOutTasks);
            }

            if (zoneId == null)
            {
                return characters;
            }

            var now = DateTime.UtcNow;
            return characters.Where(a => a.LastSeenZoneId == zoneId && now - a.LastSeenTimestamp <= LastSeenLifetime);
        }

        public async Task<bool> IsCharacterOnlineAsync(string characterId)
        {
            var onlineCharacter = await GetAsync(characterId);
            return onlineCharacter != null;
        }

        public async Task<OnlineCharacter> SetCharacterOfflineAsync(string characterId, DateTime timestamp)
        {
            var onlineCharacter = await GetAsync(characterId);
            if (onlineCharacter == null)
            {
                return null;
            }

            onlineCharacter.LogoutTimestamp = timestamp;

            await RemoveFromCacheList(onlineCharacter);

            return onlineCharacter;
        }

        public async Task<OnlineCharacter> SetCharacterOnlineAsync(string characterId, int worldId, int factionId, string outfitId, DateTime timestamp)
        {
            var onlineCharacter = new OnlineCharacter
            {
                CharacterId = characterId,
                WorldId = worldId,
                FactionId = factionId,
                OutfitId = outfitId,
                LoginTimestamp = timestamp
            };

            await Task.WhenAll(
                _cache.AddToListAsync(GetListCacheKey(worldId), characterId),
                _cache.SetAsync(GetPlayerCacheKey(characterId), onlineCharacter, CacheIdleDuration));

            return onlineCharacter;
        }

        public async Task<OnlineCharacter> UpdateLastSeenAsync(string characterId, int worldId, int factionId, string outfitId, int zoneId, DateTime timestamp)
        {
            var onlineCharacter = await GetAsync(characterId);
            if (onlineCharacter == null)
            {
                onlineCharacter = await SetCharacterOnlineAsync(characterId, worldId, factionId, outfitId, timestamp);
            }

            onlineCharacter.OutfitId = outfitId;
            onlineCharacter.LastSeenZoneId = zoneId;
            onlineCharacter.LastSeenTimestamp = timestamp;

            await _cache.SetAsync(GetPlayerCacheKey(characterId), onlineCharacter, CacheIdleDuration);

            return onlineCharacter;
        }

        public Task<OnlineCharacter> GetAsync(string characterId)
        {
            return _cache.GetAsync<OnlineCharacter>(GetPlayerCacheKey(characterId));
        }

        private Task RemoveFromCacheList(OnlineCharacter character)
        {
            return Task.WhenAll(
                _cache.RemoveFromListAsync(GetListCacheKey(character.WorldId), character.CharacterId),
                _cache.RemoveAsync(GetPlayerCacheKey(character.CharacterId)));
        }

        public Task<long> GetPlayerCountAsync(int worldId)
        {
            return _cache.GetListLengthAsync(GetListCacheKey(worldId));
        }
    }
}

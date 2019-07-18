using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterSessionService : ICharacterSessionService
    {
        private readonly IPlayerSessionRepository _playerSessionRepository;
        private readonly IWorldEventsService _worldEventService;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.sessions";

        private readonly Func<string, string> _getSessionsListCacheKey = characterId => $"{_cacheKey}_sessions_{characterId}";
        private readonly Func<string, string> _getLiveSessionCacheKey = characterId => $"{_cacheKey}_livesession_{characterId}";
        private readonly Func<string, int, string> _getSessionCacheKey = (characterId, sessionId) => $"{_cacheKey}_session_{characterId}_{sessionId}";

        private readonly TimeSpan _cacheCharacterSessionsListExpiration = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _cacheCharacterSessionExpiration = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _cacheCharacterLiveSessionExpiration = TimeSpan.FromSeconds(10);

        private readonly KeyedSemaphoreSlim _characterSessionLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _characterLiveSessionLock = new KeyedSemaphoreSlim();

        public CharacterSessionService(IPlayerSessionRepository playerSessionRepository, IWorldEventsService worldEventService, ICache cache)
        {
            _playerSessionRepository = playerSessionRepository;
            _worldEventService = worldEventService;
            _cache = cache;
        }

        public async Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetSessions(string characterId, int limit = 25, int page = 0)
        {
            var cacheKey = _getSessionsListCacheKey(characterId);

            var sessions = await _cache.GetAsync<IEnumerable<Data.Models.Planetside.PlayerSession>>(cacheKey);
            if (sessions != null)
            {
                return sessions;
            }

            sessions = await _playerSessionRepository.GetPlayerSessionsByCharacterIdAsync(characterId, limit, page);

            await _cache.SetAsync(cacheKey, sessions, _cacheCharacterSessionsListExpiration);

            return sessions;
        }

        public async Task<PlayerSession> GetSession(string characterId, int sessionId)
        {
            using (_characterSessionLock.WaitAsync($"{characterId}_{sessionId}"))
            {
                var cacheKey = _getSessionCacheKey(characterId, sessionId);

                var sessionInfo = await _cache.GetAsync<PlayerSession>(cacheKey);
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

                sessionInfo = new PlayerSession
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

                await _cache.SetAsync(cacheKey, sessionInfo, _cacheCharacterSessionExpiration);

                return sessionInfo;
            }
        }

        public async Task<PlayerSession> GetSession(string characterId)
        {
            using (_characterLiveSessionLock.WaitAsync(characterId))
            {
                var cacheKey = _getLiveSessionCacheKey(characterId);

                var sessionInfo = await _cache.GetAsync<PlayerSession>(cacheKey);
                if (sessionInfo != null)
                {
                    return sessionInfo;
                }

                var lastLoginTask = _worldEventService.GetLastPlayerLoginEventAsync(characterId);
                var lastLogoutTask = _worldEventService.GetLastPlayerLogoutEventAsync(characterId);

                await Task.WhenAll(lastLoginTask, lastLogoutTask);

                var lastLogin = lastLoginTask.Result;
                var lastLogout = lastLogoutTask.Result;

                if (lastLogin == null || (lastLogout != null && lastLogout.Timestamp >= lastLogin.Timestamp) || (DateTime.UtcNow - lastLogin.Timestamp > TimeSpan.FromHours(48)))
                {
                    return null;
                }

                var sessionEvents = await GetSessionEventsForCharacter(characterId, lastLogin.Timestamp, DateTime.UtcNow);

                sessionEvents.Insert(0, new PlayerSessionLoginEvent { Timestamp = lastLogin.Timestamp });

                sessionInfo = new PlayerSession
                {
                    Events = sessionEvents,
                    Session = new PlayerSessionInfo
                    {
                        CharacterId = characterId,
                        Duration = (int)(DateTime.UtcNow - lastLogin.Timestamp).TotalMilliseconds,
                        LoginDate = lastLogin.Timestamp
                    }
                };

                await _cache.SetAsync(cacheKey, sessionInfo, _cacheCharacterLiveSessionExpiration);

                return sessionInfo;
            }
        }

        private async Task<List<PlayerSessionEvent>> GetSessionEventsForCharacter(string characterId, DateTime start, DateTime end)
        {
            var sessionDeathsTask = _worldEventService.GetDeathEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionFacilityCapturesTask = _worldEventService.GetFacilityCaptureEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionFacilityDefendsTask = _worldEventService.GetFacilityDefendEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionBattleRankUpsTask = _worldEventService.GetBattleRankUpEventsForCharacterIdByDateAsync(characterId, start, end);
            var sessionVehicleDestroysTask = _worldEventService.GetVehicleDestroyEventsForCharacterIdByDateAsync(characterId, start, end);

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
    }
}

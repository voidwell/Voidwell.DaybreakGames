using AsyncKeyedLock;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

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

    private readonly AsyncKeyedLocker<string> _characterSessionLock = new();
    private readonly AsyncKeyedLocker<string> _characterLiveSessionLock = new();

    public CharacterSessionService(IPlayerSessionRepository playerSessionRepository, IWorldEventsService worldEventService, ICache cache)
    {
        _playerSessionRepository = playerSessionRepository;
        _worldEventService = worldEventService;
        _cache = cache;
    }

    public async Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetSessions(string characterId, int limit = 25, int page = 0)
    {
        var cacheKey = _getSessionsListCacheKey(characterId);

        return await _cache.GetOrSetAsync<IEnumerable<Data.Models.Planetside.PlayerSession>>(cacheKey, ct => _playerSessionRepository.GetPlayerSessionsByCharacterIdAsync(characterId, limit, page), _cacheCharacterSessionsListExpiration);
    }

    public async Task<PlayerSession?> GetSession(string characterId, int sessionId)
    {
        using (await _characterSessionLock.LockAsync($"{characterId}_{sessionId}"))
        {
            var cacheKey = _getSessionCacheKey(characterId, sessionId);

            return await _cache.GetOrSetIfNotNullAsync<PlayerSession>(cacheKey, async ct =>
            {
                var playerSession = await _playerSessionRepository.GetPlayerSessionAsync(sessionId);
                if (playerSession == null)
                {
                    return null;
                }

                var sessionEvents = await GetSessionEventsForCharacterAsync(characterId, playerSession.LoginDate, playerSession.LogoutDate);

                sessionEvents.Insert(0, new PlayerSessionLoginEvent { Timestamp = playerSession.LoginDate });
                sessionEvents.Add(new PlayerSessionLogoutEvent { Timestamp = playerSession.LogoutDate });

                var sessionInfo = new PlayerSession
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

                return sessionInfo;
            }, _cacheCharacterSessionExpiration);
        }
    }

    public async Task<PlayerSession?> GetSession(string characterId)
    {
        using (await _characterLiveSessionLock.LockAsync(characterId))
        {
            var cacheKey = _getLiveSessionCacheKey(characterId);

            return await _cache.GetOrSetIfNotNullAsync<PlayerSession>(cacheKey, async ct =>
            {
                var lastLoginTask = _worldEventService.GetLastPlayerLoginEventAsync(characterId);
                var lastLogoutTask = _worldEventService.GetLastPlayerLogoutEventAsync(characterId);

                await Task.WhenAll(lastLoginTask, lastLogoutTask);

                var lastLogin = lastLoginTask.Result;
                var lastLogout = lastLogoutTask.Result;

                if (lastLogin == null || (lastLogout != null && lastLogout.Timestamp >= lastLogin.Timestamp) || DateTime.UtcNow - lastLogin.Timestamp > TimeSpan.FromHours(24))
                {
                    return null;
                }

                var sessionEvents = await GetSessionEventsForCharacterAsync(characterId, lastLogin.Timestamp, DateTime.UtcNow);

                sessionEvents.Insert(0, new PlayerSessionLoginEvent { Timestamp = lastLogin.Timestamp });

                var sessionInfo = new PlayerSession
                {
                    Events = sessionEvents,
                    Session = new PlayerSessionInfo
                    {
                        CharacterId = characterId,
                        Duration = (int)(DateTime.UtcNow - lastLogin.Timestamp).TotalMilliseconds,
                        LoginDate = lastLogin.Timestamp
                    }
                };

                return sessionInfo;
            }, _cacheCharacterLiveSessionExpiration);
        }
    }

    private async Task<List<PlayerSessionEvent>> GetSessionEventsForCharacterAsync(string characterId, DateTime start, DateTime end)
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

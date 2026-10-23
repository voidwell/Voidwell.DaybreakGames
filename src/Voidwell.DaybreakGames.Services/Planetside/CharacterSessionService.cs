using System.Globalization;
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

    private readonly Func<long, int, int, string> _getSessionsListCacheKey = (characterId, page, limit) => $"{_cacheKey}_sessions_{characterId}_{page}_{limit}";
    private readonly Func<long, string> _getLiveSessionCacheKey = characterId => $"{_cacheKey}_livesession_{characterId}";
    private readonly Func<long, int, string> _getSessionCacheKey = (characterId, sessionId) => $"{_cacheKey}_session_{characterId}_{sessionId}";

    private readonly TimeSpan _cacheCharacterSessionsListExpiration = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _cacheCharacterSessionExpiration = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _cacheCharacterLiveSessionExpiration = TimeSpan.FromSeconds(10);

    private readonly AsyncKeyedLocker<string> _characterSessionLock = new();
    private readonly AsyncKeyedLocker<long> _characterLiveSessionLock = new();

    public CharacterSessionService(IPlayerSessionRepository playerSessionRepository, IWorldEventsService worldEventService, ICache cache)
    {
        _playerSessionRepository = playerSessionRepository;
        _worldEventService = worldEventService;
        _cache = cache;
    }

    public async Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetCharacterSessionsAsync(long characterId, CancellationToken cancellationToken, int limit = 25, int page = 0)
    {
        return await _cache.GetOrSetAsync(
            _getSessionsListCacheKey(characterId, page, limit),
            ct => _playerSessionRepository.GetPlayerSessionsByCharacterIdAsync(characterId, limit, ct, page),
            _cacheCharacterSessionsListExpiration,
            cancellationToken);
    }

    public async Task<PlayerSession?> GetCharacterSessionAsync(long characterId, int sessionId, CancellationToken cancellationToken)
    {
        using var _ = await _characterSessionLock.LockAsync($"{characterId}_{sessionId}");

        return await _cache.GetOrSetIfNotNullAsync(
            _getSessionCacheKey(characterId, sessionId),
            ct => GetCharacterSessionDataAsync(characterId, sessionId, ct),
            _cacheCharacterSessionExpiration,
            cancellationToken);
    }

    private async Task<PlayerSession?> GetCharacterSessionDataAsync(long characterId, int sessionId, CancellationToken cancellationToken)
    {
        var playerSession = await _playerSessionRepository.GetPlayerSessionAsync(sessionId, cancellationToken);
        if (playerSession == null)
        {
            return null;
        }

        var sessionEvents = await GetSessionEventsForCharacterAsync(characterId, playerSession.LoginDate, playerSession.LogoutDate, cancellationToken);

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
    }

    public async Task<PlayerSession?> GetCharacterLiveSessionAsync(long characterId, CancellationToken cancellationToken)
    {
        using var _ = await _characterLiveSessionLock.LockAsync(characterId);

        var cacheKey = _getLiveSessionCacheKey(characterId);

        return await _cache.GetOrSetIfNotNullAsync<PlayerSession>(cacheKey, async ct =>
        {
            var lastLoginTask = _worldEventService.GetLastPlayerLoginEventAsync(characterId);
            var lastLogoutTask = _worldEventService.GetLastPlayerLogoutEventAsync(characterId);

            await Task.WhenAll(lastLoginTask, lastLogoutTask);

            var lastLogin = await lastLoginTask;
            var lastLogout = await lastLogoutTask;

            if (lastLogin == null || (lastLogout != null && lastLogout.Timestamp >= lastLogin.Timestamp) || DateTime.UtcNow - lastLogin.Timestamp > TimeSpan.FromHours(24))
            {
                return null;
            }

            var sessionEvents = await GetSessionEventsForCharacterAsync(characterId, lastLogin.Timestamp, DateTime.UtcNow, cancellationToken);

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

    private async Task<List<PlayerSessionEvent>> GetSessionEventsForCharacterAsync(long characterId, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        var sessionDeathsTask = _worldEventService.GetDeathEventsForCharacterIdByDateAsync(characterId, start, end);
        var sessionFacilityCapturesTask = _worldEventService.GetFacilityCaptureEventsForCharacterIdByDateAsync(characterId, start, end);
        var sessionFacilityDefendsTask = _worldEventService.GetFacilityDefendEventsForCharacterIdByDateAsync(characterId, start, end);
        var sessionBattleRankUpsTask = _worldEventService.GetBattleRankUpEventsForCharacterIdByDateAsync(characterId, start, end);
        var sessionVehicleDestroysTask = _worldEventService.GetVehicleDestroyEventsForCharacterIdByDateAsync(characterId, start, end);

        await Task.WhenAll(sessionDeathsTask, sessionFacilityCapturesTask, sessionFacilityDefendsTask, sessionBattleRankUpsTask, sessionVehicleDestroysTask);

        var sessionEvents = new List<PlayerSessionEvent>();

        sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(await sessionDeathsTask));
        sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(await sessionFacilityCapturesTask));
        sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(await sessionFacilityDefendsTask));
        sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(await sessionBattleRankUpsTask));
        sessionEvents.AddRange(PlayerSessionEventMapper.ToPlayerSessionEvent(await sessionVehicleDestroysTask));

        return [.. sessionEvents.OrderBy(a => a.Timestamp)];
    }

    public async Task<IEnumerable<PlayerSessionSummary>> GetLatestCharacterSessionsAsync(long characterId, int limit, CancellationToken cancellationToken)
    {
        var sessions = await GetCharacterSessionsAsync(characterId, cancellationToken, limit, page: 0);
        var details = await Task.WhenAll(sessions.Select(session => GetCharacterSessionAsync(characterId, session.Id, cancellationToken)));

        var sessionCharacterId = characterId.ToString(CultureInfo.InvariantCulture);

        return [.. details.OfType<PlayerSession>().Select(detail =>
        {
            var deathEvents = detail.Events.OfType<PlayerSessionDeathEvent>().ToList();

            return new PlayerSessionSummary
            {
                Id = detail.Session.Id,
                Duration = detail.Session.Duration,
                LoginDate = detail.Session.LoginDate,
                LogoutDate = detail.Session.LogoutDate,
                Kills = deathEvents.Count(e => e.Attacker?.Id == sessionCharacterId && e.Victim?.Id != sessionCharacterId && e.Attacker?.FactionId != e.Victim?.FactionId),
                Deaths = deathEvents.Count(e => e.Victim?.Id == sessionCharacterId)
            };
        })];
    }
}

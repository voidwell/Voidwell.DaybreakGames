using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class CharacterSessionServiceTest
{
    private const long _characterId = 12345;
    private const int _myFaction = 1;
    private const int _enemyFaction = 2;

    private static readonly DateTime _login = new(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IPlayerSessionRepository> _repository = new();
    private readonly Mock<IWorldEventsService> _worldEvents = new();
    private readonly CharacterSessionService _sut;

    public CharacterSessionServiceTest()
    {
        _sut = new CharacterSessionService(_repository.Object, _worldEvents.Object, TestCache.Create());
    }

    [Fact]
    public async Task GetLatestCharacterSessions_NoSessions_ReturnsEmpty()
    {
        SetupSessionList();

        var result = await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_MapsSessionInfo()
    {
        SetupSession(10, Array.Empty<Death>());

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal("10", summary.Id);
        Assert.Equal(3600, summary.Duration);
        Assert.Equal(LoginFor(10), summary.LoginDate);
        Assert.Equal(LogoutFor(10), summary.LogoutDate);
        Assert.Equal(0, summary.Kills);
        Assert.Equal(0, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_CountsKillsAndDeaths()
    {
        SetupSession(10,
            Kill(victimId: 1),
            Kill(victimId: 2),
            Death(attackerId: 1));

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal(2, summary.Kills);
        Assert.Equal(1, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_Suicide_CountsAsDeathNotKill()
    {
        SetupSession(10, Suicide());

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal(0, summary.Kills);
        Assert.Equal(1, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_SuicideWithoutFactionData_CountsAsDeathNotKill()
    {
        var suicide = Suicide();
        suicide.Character = null;
        suicide.AttackerCharacter = null;
        SetupSession(10, suicide);

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal(0, summary.Kills);
        Assert.Equal(1, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_EnvironmentalDeath_CountsAsDeath()
    {
        SetupSession(10, Death(attackerId: 0, attackerFaction: null));

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal(0, summary.Kills);
        Assert.Equal(1, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_Teamkill_DoesNotCountAsKill()
    {
        SetupSession(10,
            Kill(victimId: 4, victimFaction: _myFaction),
            Kill(victimId: 3));

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal(1, summary.Kills);
        Assert.Equal(0, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_KilledByTeammate_CountsAsDeath()
    {
        SetupSession(10, Death(attackerId: 4, attackerFaction: _myFaction));

        var summary = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).Single();

        Assert.Equal(0, summary.Kills);
        Assert.Equal(1, summary.Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_CountsEachSessionSeparately()
    {
        SetupSessionList(10, 11);
        SetupSessionDetails(10, Kill(victimId: 1), Kill(victimId: 2));
        SetupSessionDetails(11, Death(attackerId: 1));

        var summaries = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).ToList();

        Assert.Equal(2, summaries.Count());
        Assert.Equal(2, summaries.Single(a => a.Id == "10").Kills);
        Assert.Equal(0, summaries.Single(a => a.Id == "10").Deaths);
        Assert.Equal(0, summaries.Single(a => a.Id == "11").Kills);
        Assert.Equal(1, summaries.Single(a => a.Id == "11").Deaths);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_SessionNotFound_IsSkipped()
    {
        SetupSessionList(10, 11);
        SetupSessionDetails(10, Kill(victimId: 1));
        _repository.Setup(a => a.GetPlayerSessionAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync((PlayerSession)null);

        var summaries = (await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken)).ToList();

        Assert.Equal("10", Assert.Single(summaries).Id);
    }

    [Fact]
    public async Task GetLatestCharacterSessions_RequestsLatestFiveSessions()
    {
        SetupSessionList();

        await _sut.GetLatestCharacterSessionsAsync(_characterId, limit: 5, TestContext.Current.CancellationToken);

        _repository.Verify(a => a.GetPlayerSessionsByCharacterIdAsync(_characterId, 5, It.IsAny<CancellationToken>(), 0), Times.Once);
    }

    private void SetupSession(int sessionId, params Death[] deaths)
    {
        SetupSessionList(sessionId);
        SetupSessionDetails(sessionId, deaths);
    }

    private void SetupSessionList(params int[] sessionIds)
    {
        _repository
            .Setup(a => a.GetPlayerSessionsByCharacterIdAsync(_characterId, It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(sessionIds.Select(id => new PlayerSession { Id = id, CharacterId = _characterId }).ToList());
    }

    private void SetupSessionDetails(int sessionId, params Death[] deaths)
    {
        _repository
            .Setup(a => a.GetPlayerSessionAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlayerSession
            {
                Id = sessionId,
                CharacterId = _characterId,
                LoginDate = LoginFor(sessionId),
                LogoutDate = LogoutFor(sessionId),
                Duration = 3600
            });

        // Each session gets its own time window so its events are set up independently.
        _worldEvents
            .Setup(a => a.GetDeathEventsForCharacterIdByDateAsync(_characterId, LoginFor(sessionId), LogoutFor(sessionId)))
            .ReturnsAsync(deaths);
        _worldEvents
            .Setup(a => a.GetFacilityCaptureEventsForCharacterIdByDateAsync(_characterId, LoginFor(sessionId), LogoutFor(sessionId)))
            .ReturnsAsync(Array.Empty<PlayerFacilityCapture>());
        _worldEvents
            .Setup(a => a.GetFacilityDefendEventsForCharacterIdByDateAsync(_characterId, LoginFor(sessionId), LogoutFor(sessionId)))
            .ReturnsAsync(Array.Empty<PlayerFacilityDefend>());
        _worldEvents
            .Setup(a => a.GetBattleRankUpEventsForCharacterIdByDateAsync(_characterId, LoginFor(sessionId), LogoutFor(sessionId)))
            .ReturnsAsync(Array.Empty<BattlerankUp>());
        _worldEvents
            .Setup(a => a.GetVehicleDestroyEventsForCharacterIdByDateAsync(_characterId, LoginFor(sessionId), LogoutFor(sessionId)))
            .ReturnsAsync(Array.Empty<VehicleDestroy>());
    }

    private static DateTime LoginFor(int sessionId) => _login.AddDays(sessionId);

    private static DateTime LogoutFor(int sessionId) => LoginFor(sessionId).AddHours(1);

    /// <summary>The character kills <paramref name="victimId"/>; defaults to an enemy-faction victim.</summary>
    private static Death Kill(long victimId, int victimFaction = _enemyFaction)
    {
        return new Death
        {
            Timestamp = _login.AddMinutes(1),
            CharacterId = victimId,
            Character = new Character { FactionId = victimFaction },
            AttackerCharacterId = _characterId,
            AttackerCharacter = new Character { FactionId = _myFaction },
            AttackerWeaponId = 80
        };
    }

    /// <summary>The character is killed by <paramref name="attackerId"/>; defaults to an enemy-faction attacker.</summary>
    private static Death Death(long attackerId, int? attackerFaction = _enemyFaction)
    {
        return new Death
        {
            Timestamp = _login.AddMinutes(2),
            CharacterId = _characterId,
            Character = new Character { FactionId = _myFaction },
            AttackerCharacterId = attackerId,
            AttackerCharacter = attackerFaction.HasValue ? new Character { FactionId = attackerFaction.Value } : null,
            AttackerWeaponId = 80
        };
    }

    private static Death Suicide()
    {
        return new Death
        {
            Timestamp = _login.AddMinutes(3),
            CharacterId = _characterId,
            Character = new Character { FactionId = _myFaction },
            AttackerCharacterId = _characterId,
            AttackerCharacter = new Character { FactionId = _myFaction },
            AttackerWeaponId = 80
        };
    }
}

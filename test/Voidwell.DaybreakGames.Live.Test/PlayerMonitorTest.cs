using Moq;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class PlayerMonitorTest
{
    private const int _worldId = 17;
    private static readonly DateTime _login = new(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<ICharacterService> _characterService = new();
    private readonly Mock<ICharacterUpdaterService> _updaterService = new();
    private readonly Mock<IPlayerSessionRepository> _sessionRepository = new();
    private readonly Mock<ICharacterRatingService> _ratingService = new();
    private readonly ICache _cache = TestCache.Create();

    private PlayerMonitor CreateSut()
    {
        _updaterService.Setup(a => a.AddToQueue(It.IsAny<long>())).Returns(Task.CompletedTask);
        _sessionRepository.Setup(a => a.AddAsync(It.IsAny<PlayerSession>())).Returns(Task.CompletedTask);
        _ratingService.Setup(a => a.SaveCachedRatingAsync(It.IsAny<long>())).Returns(Task.CompletedTask);

        return new PlayerMonitor(_characterService.Object, _updaterService.Object, _sessionRepository.Object, _ratingService.Object, _cache);
    }

    private void GivenCharacter(long id, int worldId = _worldId, string name = null)
    {
        _characterService.Setup(a => a.GetCharacter(id)).ReturnsAsync(new Character { Id = id, Name = name ?? id.ToString(), FactionId = 2, WorldId = worldId });
    }

    [Fact]
    public async Task SetOnlineAsync_UnknownCharacter_ReturnsNull()
    {
        _characterService.Setup(a => a.GetCharacter(999)).ReturnsAsync((Character)null);

        Assert.Null((await CreateSut().SetOnlineAsync(999, _login)));
    }

    [Fact]
    public async Task SetOnlineAsync_KnownCharacter_TracksPlayerAndCountsThemOnTheWorld()
    {
        GivenCharacter(1, name: "Bob");
        var sut = CreateSut();

        var online = await sut.SetOnlineAsync(1, _login);

        Assert.Equal(1, online.Character.CharacterId);
        Assert.Equal("Bob", online.Character.Name);
        Assert.Equal(2, online.Character.FactionId);
        Assert.Equal(_worldId, online.Character.WorldId);
        Assert.Equal(_login, online.LoginDate);
        Assert.Equivalent(online, await sut.GetAsync(1));
        Assert.Equal(1, (await sut.GetPlayerCountAsync(_worldId)));
    }

    [Fact]
    public async Task SetOfflineAsync_UnknownCharacter_ReturnsNull()
    {
        _characterService.Setup(a => a.GetCharacter(999)).ReturnsAsync((Character)null);

        Assert.Null((await CreateSut().SetOfflineAsync(999, _login)));
    }

    [Fact]
    public async Task SetOfflineAsync_CharacterWasNotOnline_ReturnsNullAndRecordsNothing()
    {
        GivenCharacter(1);

        var result = await CreateSut().SetOfflineAsync(1, _login);

        Assert.Null(result);
        _sessionRepository.Verify(a => a.AddAsync(It.IsAny<PlayerSession>()), Times.Never);
    }

    [Fact]
    public async Task SetOfflineAsync_LongSession_RecordsSessionQueuesUpdateAndSavesRating()
    {
        GivenCharacter(1);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);

        var result = await sut.SetOfflineAsync(1, _login.AddMinutes(30));

        Assert.NotNull(result);
        _sessionRepository.Verify(a => a.AddAsync(It.Is<PlayerSession>(s =>
            s.CharacterId == 1 && s.LoginDate == _login && s.LogoutDate == _login.AddMinutes(30) && s.Duration == 30 * 60 * 1000)), Times.Once);
        _updaterService.Verify(a => a.AddToQueue(1), Times.Once);
        _ratingService.Verify(a => a.SaveCachedRatingAsync(1), Times.Once);
    }

    [Fact]
    public async Task SetOfflineAsync_ShortSession_DoesNotQueueCharacterUpdate()
    {
        GivenCharacter(1);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);

        await sut.SetOfflineAsync(1, _login.AddMinutes(4));

        _updaterService.Verify(a => a.AddToQueue(It.IsAny<long>()), Times.Never);
        _sessionRepository.Verify(a => a.AddAsync(It.IsAny<PlayerSession>()), Times.Once);
    }

    [Fact]
    public async Task SetOfflineAsync_RemovesPlayerFromWorldTracking()
    {
        GivenCharacter(1);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);

        await sut.SetOfflineAsync(1, _login.AddMinutes(10));

        Assert.Null((await sut.GetAsync(1)));
        Assert.Equal(0, (await sut.GetPlayerCountAsync(_worldId)));
    }

    [Fact]
    public async Task SetLastSeenAsync_PlayerNotOnline_SetsThemOnlineFirst()
    {
        GivenCharacter(1);
        var sut = CreateSut();

        var result = await sut.SetLastSeenAsync(1, zoneId: 4, _login);

        Assert.Equal(_login, result.LoginDate);
        Assert.Equal(4, result.LastSeen.ZoneId);
        Assert.Equal(_login, result.LastSeen.Timestamp);
        Assert.Equal(1, (await sut.GetPlayerCountAsync(_worldId)));
    }

    [Fact]
    public async Task SetLastSeenAsync_PlayerOnline_UpdatesLastSeenAndKeepsLoginDate()
    {
        GivenCharacter(1);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);

        await sut.SetLastSeenAsync(1, zoneId: 6, _login.AddMinutes(5));

        var stored = await sut.GetAsync(1);
        Assert.Equal(_login, stored.LoginDate);
        Assert.Equal(6, stored.LastSeen.ZoneId);
        Assert.Equal(_login.AddMinutes(5), stored.LastSeen.Timestamp);
    }

    [Fact]
    public async Task SetLastSeenAsync_UnknownCharacter_ReturnsNull()
    {
        _characterService.Setup(a => a.GetCharacter(999)).ReturnsAsync((Character)null);

        Assert.Null((await CreateSut().SetLastSeenAsync(999, 2, _login)));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlinePlayersForWorld()
    {
        GivenCharacter(1);
        GivenCharacter(2);
        GivenCharacter(3, worldId: 99);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);
        await sut.SetOnlineAsync(2, _login);
        await sut.SetOnlineAsync(3, _login);

        var players = await sut.GetAllAsync(_worldId);

        Assert.Equivalent(new long?[] { 1, 2 }, players.Select(a => a.Character.CharacterId));
    }

    [Fact]
    public async Task GetAllAsync_PlayersWhoseCacheEntryExpired_AreRemovedFromTheList()
    {
        GivenCharacter(1);
        GivenCharacter(2);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);
        await sut.SetOnlineAsync(2, _login);
        await _cache.RemoveAsync("ps2.online-players_character_2");

        var players = await sut.GetAllAsync(_worldId);

        Assert.Equal(new long?[] { 1 }, players.Select(a => a.Character.CharacterId));
        Assert.Equal(1, (await sut.GetPlayerCountAsync(_worldId)));
    }

    [Fact]
    public async Task GetAllAsync_ZoneFilter_OnlyReturnsRecentlySeenPlayersInZone()
    {
        GivenCharacter(11);
        GivenCharacter(12);
        GivenCharacter(13);
        var sut = CreateSut();
        await sut.SetLastSeenAsync(11, 2, DateTime.UtcNow.AddMinutes(-1));
        await sut.SetLastSeenAsync(12, 2, DateTime.UtcNow.AddMinutes(-30));
        await sut.SetLastSeenAsync(13, 4, DateTime.UtcNow.AddMinutes(-1));

        var players = await sut.GetAllAsync(_worldId, zoneId: 2);

        Assert.Equal(new long?[] { 11 }, players.Select(a => a.Character.CharacterId));
    }

    [Fact]
    public async Task ClearWorldAsync_RemovesWorldPlayerList()
    {
        GivenCharacter(1);
        var sut = CreateSut();
        await sut.SetOnlineAsync(1, _login);

        await sut.ClearWorldAsync(_worldId);

        Assert.Equal(0, (await sut.GetPlayerCountAsync(_worldId)));
    }
}

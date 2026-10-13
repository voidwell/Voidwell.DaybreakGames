using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class ServiceCachingTest
{
    [Fact]
    public async Task AlertService_GetAlerts_CachesNonEmptyResultsPerPageAndWorld()
    {
        var repository = new Mock<IAlertRepository>();
        repository.Setup(a => a.GetAlerts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>())).ReturnsAsync(new[] { new Alert { MetagameInstanceId = 1 } });
        var sut = new AlertService(repository.Object, null, null, TestCache.Create());

        await sut.GetAlerts(0, 17);
        await sut.GetAlerts(0, 17);
        await sut.GetAlerts(1, 17);
        await sut.GetAlerts(0, null);

        repository.Verify(a => a.GetAlerts(0, 10, 17), Times.Once);
        repository.Verify(a => a.GetAlerts(1, 10, 17), Times.Once);
        repository.Verify(a => a.GetAlerts(0, 10, null), Times.Once);
    }

    [Fact]
    public async Task AlertService_GetAlerts_DoesNotCacheEmptyResults()
    {
        var repository = new Mock<IAlertRepository>();
        repository.Setup(a => a.GetAlerts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>())).ReturnsAsync(Array.Empty<Alert>());
        var sut = new AlertService(repository.Object, null, null, TestCache.Create());

        Assert.Empty((await sut.GetAlerts(0, 17)));
        await sut.GetAlerts(0, 17);

        repository.Verify(a => a.GetAlerts(0, 10, 17), Times.Exactly(2));
    }

    [Fact]
    public async Task AlertService_GetActiveAlertsByWorldId_BypassesCache()
    {
        var repository = new Mock<IAlertRepository>();
        repository.Setup(a => a.GetActiveAlertsByWorldId(17)).ReturnsAsync(Array.Empty<Alert>());
        var sut = new AlertService(repository.Object, null, null, TestCache.Create());

        await sut.GetActiveAlertsByWorldId(17);
        await sut.GetActiveAlertsByWorldId(17);

        repository.Verify(a => a.GetActiveAlertsByWorldId(17), Times.Exactly(2));
    }

    [Fact]
    public async Task WeaponAggregateService_CachesFoundAggregatePerItem()
    {
        var repository = new Mock<IWeaponAggregateRepository>();
        repository.Setup(a => a.GetWeaponAggregateByItemId(It.IsAny<int>())).ReturnsAsync((int id) => new WeaponAggregate { ItemId = id });
        var sut = new WeaponAggregateService(repository.Object, TestCache.Create());

        var first = await sut.GetAggregateForItem(80);
        var second = await sut.GetAggregateForItem(80);
        await sut.GetAggregateForItem(81);

        Assert.Same(first, second);
        repository.Verify(a => a.GetWeaponAggregateByItemId(80), Times.Once);
        repository.Verify(a => a.GetWeaponAggregateByItemId(81), Times.Once);
    }

    [Fact]
    public async Task WeaponAggregateService_DoesNotCacheMissingAggregate()
    {
        var repository = new Mock<IWeaponAggregateRepository>();
        repository.Setup(a => a.GetWeaponAggregateByItemId(It.IsAny<int>())).ReturnsAsync((WeaponAggregate)null);
        var sut = new WeaponAggregateService(repository.Object, TestCache.Create());

        Assert.Null((await sut.GetAggregateForItem(80)));
        await sut.GetAggregateForItem(80);

        repository.Verify(a => a.GetWeaponAggregateByItemId(80), Times.Exactly(2));
    }

    [Fact]
    public async Task WeaponAggregateService_GetAggregates_KeysByItemAndVehicleAndSkipsMissing()
    {
        var repository = new Mock<IWeaponAggregateRepository>();
        repository.Setup(a => a.GetWeaponAggregateByItemId(80)).ReturnsAsync(new WeaponAggregate { ItemId = 80, VehicleId = 0 });
        repository.Setup(a => a.GetWeaponAggregateByItemId(81)).ReturnsAsync(new WeaponAggregate { ItemId = 81, VehicleId = 5 });
        repository.Setup(a => a.GetWeaponAggregateByItemId(82)).ReturnsAsync((WeaponAggregate)null);
        var sut = new WeaponAggregateService(repository.Object, TestCache.Create());

        var result = await sut.GetAggregates(new[] { 80, 81, 82, 80 });

        Assert.Equivalent(new[] { "80-0", "81-5" }, result.Keys);
    }

    [Fact]
    public async Task CharacterRatingService_GetRating_UnknownCharacter_ReturnsDefaultRatingAndCachesIt()
    {
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(a => a.GetCharacterRatingAsync("c1")).ReturnsAsync((CharacterRating)null);
        var sut = new CharacterRatingService(repository.Object, TestCache.Create());

        var rating = await sut.GetRatingAsync("c1");
        var again = await sut.GetRatingAsync("c1");

        Assert.Equal("c1", rating.CharacterId);
        Assert.Equal(1500, rating.Rating);
        Assert.Equal(100, rating.Deviation);
        Assert.Equal(0.02, rating.Volatility);
        Assert.Same(rating, again);
        repository.Verify(a => a.GetCharacterRatingAsync("c1"), Times.Once);
    }

    [Fact]
    public async Task CharacterRatingService_GetRating_UsesStoredRating()
    {
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(a => a.GetCharacterRatingAsync("c1")).ReturnsAsync(new CharacterRating { CharacterId = "c1", Rating = 1710 });
        var sut = new CharacterRatingService(repository.Object, TestCache.Create());

        Assert.Equal(1710, (await sut.GetRatingAsync("c1")).Rating);
    }

    [Fact]
    public async Task CharacterRatingService_SaveCachedRating_PersistsAndEvictsCachedRating()
    {
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(a => a.GetCharacterRatingAsync("c1")).ReturnsAsync((CharacterRating)null);
        repository.Setup(a => a.UpsertAsync(It.IsAny<CharacterRating>())).ReturnsAsync((CharacterRating r) => r);
        var sut = new CharacterRatingService(repository.Object, TestCache.Create());
        await sut.GetRatingAsync("c1");

        await sut.SaveCachedRatingAsync("c1");

        repository.Verify(a => a.UpsertAsync(It.Is<CharacterRating>(r => r.CharacterId == "c1")), Times.Once);

        // evicted: the next read goes back to the repository
        await sut.GetRatingAsync("c1");
        repository.Verify(a => a.GetCharacterRatingAsync("c1"), Times.Exactly(2));
    }

    [Fact]
    public async Task CharacterRatingService_SaveCachedRating_NothingCached_DoesNotPersist()
    {
        var repository = new Mock<ICharacterRepository>();
        var sut = new CharacterRatingService(repository.Object, TestCache.Create());

        await sut.SaveCachedRatingAsync("c1");

        repository.Verify(a => a.UpsertAsync(It.IsAny<CharacterRating>()), Times.Never);
    }

    [Fact]
    public async Task CharacterRatingService_Leaderboard_MapsCharacterDetailsAndCaches()
    {
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(a => a.GetCharacterRatingLeaderboardAsync(10)).ReturnsAsync(new[]
        {
            new CharacterRating { CharacterId = "c1", Rating = 1800, Deviation = 50, Character = new Character { Name = "Bob", FactionId = 2, WorldId = 17, BattleRank = 100 } },
            new CharacterRating { CharacterId = "c2", Rating = 1700, Deviation = 60 }
        });
        var sut = new CharacterRatingService(repository.Object, TestCache.Create());

        var leaderboard = (await sut.GetRatingsLeaderboardAsync(10)).ToList();
        await sut.GetRatingsLeaderboardAsync(10);

        Assert.Equal(2, leaderboard.Count());
        Assert.Equal("Bob", leaderboard[0].Name);
        Assert.Equal(2, leaderboard[0].FactionId);
        Assert.Equal(17, leaderboard[0].WorldId);
        Assert.Equal(100, leaderboard[0].BattleRank);
        Assert.Equal(1800, leaderboard[0].Rating);
        Assert.Null(leaderboard[1].Name);
        repository.Verify(a => a.GetCharacterRatingLeaderboardAsync(10), Times.Once);
    }

    [Fact]
    public async Task CharacterRatingService_Leaderboard_EmptyResultIsNotCached()
    {
        var repository = new Mock<ICharacterRepository>();
        repository.Setup(a => a.GetCharacterRatingLeaderboardAsync(10)).ReturnsAsync(Array.Empty<CharacterRating>());
        var sut = new CharacterRatingService(repository.Object, TestCache.Create());

        await sut.GetRatingsLeaderboardAsync(10);
        await sut.GetRatingsLeaderboardAsync(10);

        repository.Verify(a => a.GetCharacterRatingLeaderboardAsync(10), Times.Exactly(2));
    }
}

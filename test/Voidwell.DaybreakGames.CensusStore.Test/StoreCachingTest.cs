using FluentAssertions;
using Moq;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class StoreCachingTest
{
    [Fact]
    public async Task FactionStore_CachesFoundFaction()
    {
        var repository = new Mock<IFactionRepository>();
        repository.Setup(a => a.GetFactionByIdAsync(1)).ReturnsAsync(new Faction { Id = 1 });
        var sut = new FactionStore(repository.Object, TestCache.Create());

        var first = await sut.GetFactionByIdAsync(1);
        var second = await sut.GetFactionByIdAsync(1);

        first.Id.Should().Be(1);
        second.Should().BeSameAs(first);
        repository.Verify(a => a.GetFactionByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task FactionStore_DoesNotCacheMissingFaction()
    {
        var repository = new Mock<IFactionRepository>();
        repository.Setup(a => a.GetFactionByIdAsync(99)).ReturnsAsync((Faction)null);
        var sut = new FactionStore(repository.Object, TestCache.Create());

        (await sut.GetFactionByIdAsync(99)).Should().BeNull();
        (await sut.GetFactionByIdAsync(99)).Should().BeNull();

        repository.Verify(a => a.GetFactionByIdAsync(99), Times.Exactly(2));
    }

    [Fact]
    public async Task FactionStore_CachesPerFactionId()
    {
        var repository = new Mock<IFactionRepository>();
        repository.Setup(a => a.GetFactionByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => new Faction { Id = id });
        var sut = new FactionStore(repository.Object, TestCache.Create());

        (await sut.GetFactionByIdAsync(1)).Id.Should().Be(1);
        (await sut.GetFactionByIdAsync(2)).Id.Should().Be(2);

        repository.Verify(a => a.GetFactionByIdAsync(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ZoneStore_GetZoneAsync_CachesFoundZone()
    {
        var repository = new Mock<IZoneRepository>();
        repository.Setup(a => a.GetZonesByIdsAsync(2)).ReturnsAsync(new[] { new Zone { Id = 2 } });
        var sut = new ZoneStore(repository.Object, TestCache.Create());

        var first = await sut.GetZoneAsync(2);
        var second = await sut.GetZoneAsync(2);

        first.Id.Should().Be(2);
        second.Should().BeSameAs(first);
        repository.Verify(a => a.GetZonesByIdsAsync(2), Times.Once);
    }

    [Fact]
    public async Task ZoneStore_GetZoneAsync_DoesNotCacheMissingZone()
    {
        var repository = new Mock<IZoneRepository>();
        repository.Setup(a => a.GetZonesByIdsAsync(It.IsAny<int[]>())).ReturnsAsync(Array.Empty<Zone>());
        var sut = new ZoneStore(repository.Object, TestCache.Create());

        (await sut.GetZoneAsync(5)).Should().BeNull();
        (await sut.GetZoneAsync(5)).Should().BeNull();

        repository.Verify(a => a.GetZonesByIdsAsync(5), Times.Exactly(2));
    }

    [Fact]
    public async Task ZoneStore_GetPlayableZones_RequestsPlayableZoneIdsOnceAndCaches()
    {
        var repository = new Mock<IZoneRepository>();
        repository.Setup(a => a.GetZonesByIdsAsync(It.IsAny<int[]>())).ReturnsAsync(new[] { new Zone { Id = 2 } });
        var sut = new ZoneStore(repository.Object, TestCache.Create());

        await sut.GetPlayableZones();
        var zones = await sut.GetPlayableZones();

        zones.Should().ContainSingle();
        repository.Verify(a => a.GetZonesByIdsAsync(new[] { 2, 4, 6, 8, 344 }), Times.Once);
    }

    [Fact]
    public async Task ZoneStore_GetAllZones_BypassesCache()
    {
        var repository = new Mock<IZoneRepository>();
        repository.Setup(a => a.GetAllZonesAsync()).ReturnsAsync(Array.Empty<Zone>());
        var sut = new ZoneStore(repository.Object, TestCache.Create());

        await sut.GetAllZones();
        await sut.GetAllZones();

        repository.Verify(a => a.GetAllZonesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task WorldStore_GetAllWorlds_Caches()
    {
        var repository = new Mock<IWorldRepository>();
        repository.Setup(a => a.GetAllWorldsAsync()).ReturnsAsync(new[] { new World { Id = 1 } });
        var sut = new WorldStore(repository.Object, TestCache.Create());

        await sut.GetAllWorlds();
        var worlds = await sut.GetAllWorlds();

        worlds.Should().ContainSingle();
        repository.Verify(a => a.GetAllWorldsAsync(), Times.Once);
    }

    [Fact]
    public async Task WorldStore_GetWorldPopulationHistory_CachesPerWorldAndDateRange()
    {
        var repository = new Mock<IWorldRepository>();
        repository.Setup(a => a.GetDailyPopulationsByWorldIdAsync(It.IsAny<int>())).ReturnsAsync(new List<DailyPopulation>());
        var sut = new WorldStore(repository.Object, TestCache.Create());
        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2024, 1, 31);

        await sut.GetWorldPopulationHistory(1, start, end);
        await sut.GetWorldPopulationHistory(1, start, end);
        await sut.GetWorldPopulationHistory(2, start, end);
        await sut.GetWorldPopulationHistory(1, start, end.AddDays(-1));

        repository.Verify(a => a.GetDailyPopulationsByWorldIdAsync(1), Times.Exactly(2));
        repository.Verify(a => a.GetDailyPopulationsByWorldIdAsync(2), Times.Once);
    }

    [Fact]
    public async Task ItemStore_GetItemsByCategoryIds_CachesPerCategorySet()
    {
        var repository = new Mock<IItemRepository>();
        repository.Setup(a => a.GetItemsByCategoryIds(It.IsAny<IEnumerable<int>>())).ReturnsAsync(new[] { new Item { Id = 1 } });
        var sut = new ItemStore(repository.Object, null, TestCache.Create());

        await sut.GetItemsByCategoryIdsAsync(new[] { 1, 2 });
        await sut.GetItemsByCategoryIdsAsync(new[] { 1, 2 });
        await sut.GetItemsByCategoryIdsAsync(new[] { 3 });

        repository.Verify(a => a.GetItemsByCategoryIds(It.Is<IEnumerable<int>>(c => c.SequenceEqual(new[] { 1, 2 }))), Times.Once);
        repository.Verify(a => a.GetItemsByCategoryIds(It.Is<IEnumerable<int>>(c => c.SequenceEqual(new[] { 3 }))), Times.Once);
    }
}

using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class WeaponAggregateServiceTest
{
    private readonly Mock<IWeaponAggregateRepository> _repository = new();
    private readonly List<int[]> _queries = [];

    private WeaponAggregateService CreateSut(Func<int, WeaponAggregate> aggregateFor = null)
    {
        aggregateFor ??= itemId => new WeaponAggregate { ItemId = itemId, VehicleId = 0 };

        _repository.Setup(a => a.GetWeaponAggregatesByItemIds(It.IsAny<int[]>())).ReturnsAsync((int[] itemIds) =>
        {
            _queries.Add(itemIds);
            return itemIds.Select(aggregateFor).Where(a => a != null).ToList().AsEnumerable();
        });

        return new WeaponAggregateService(_repository.Object, TestCache.Create());
    }

    [Fact]
    public async Task GetAggregates_LoadsEveryItemInASingleQuery()
    {
        var sut = CreateSut();

        var result = await sut.GetAggregates(Enumerable.Range(1, 60));

        Assert.Equal(60, result.Count);
        var query = Assert.Single(_queries);
        Assert.Equal(60, query.Length);
    }

    [Fact]
    public async Task GetAggregates_OnlyQueriesItemsThatAreNotCached()
    {
        var sut = CreateSut();

        await sut.GetAggregates([1, 2, 3]);
        var result = await sut.GetAggregates([2, 3, 4]);

        Assert.Equal(3, result.Count);
        Assert.Equal(2, _queries.Count);
        Assert.Equal([4], _queries[1]);
    }

    [Fact]
    public async Task GetAggregates_WhenEverythingIsCached_DoesNotQuery()
    {
        var sut = CreateSut();

        await sut.GetAggregates([1, 2]);
        await sut.GetAggregates([1, 2]);

        Assert.Single(_queries);
    }

    [Fact]
    public async Task GetAggregates_KeysByItemAndVehicle_AndSkipsItemsWithoutAnAggregate()
    {
        var sut = CreateSut(itemId => itemId switch
        {
            80 => new WeaponAggregate { ItemId = 80, VehicleId = 0 },
            81 => new WeaponAggregate { ItemId = 81, VehicleId = 5 },
            _ => null
        });

        var result = await sut.GetAggregates([80, 81, 82, 80]);

        Assert.Equivalent(new[] { "80-0", "81-5" }, result.Keys);
        Assert.Equal([80, 81, 82], _queries.Single().Order());
    }

    [Fact]
    public async Task GetAggregates_DoesNotCacheItemsWithoutAnAggregate()
    {
        var sut = CreateSut(itemId => null);

        await sut.GetAggregates([82]);
        await sut.GetAggregates([82]);

        Assert.Equal(2, _queries.Count);
    }

    [Fact]
    public async Task GetAggregates_WithSeveralRowsForAnItem_UsesOneOfThem()
    {
        // weapon_aggregate is keyed by (ItemId, VehicleId); the service has always used a single row per item.
        _repository.Setup(a => a.GetWeaponAggregatesByItemIds(It.IsAny<int[]>())).ReturnsAsync(new[]
        {
            new WeaponAggregate { ItemId = 7, VehicleId = 0 },
            new WeaponAggregate { ItemId = 7, VehicleId = 3 }
        });
        var sut = new WeaponAggregateService(_repository.Object, TestCache.Create());

        var result = await sut.GetAggregates([7]);

        Assert.Single(result);
    }
}

using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class WeaponAggregateServiceTest
{
    [Fact]
    public async Task GetAggregates_LimitsConcurrentDatabaseLookups()
    {
        // Every cache miss opens a database connection, so unbounded fan-out can exhaust the connection pool.
        var concurrent = 0;
        var maxConcurrent = 0;

        var repository = new Mock<IWeaponAggregateRepository>();
        repository.Setup(a => a.GetWeaponAggregateByItemId(It.IsAny<int>())).Returns(async (int itemId) =>
        {
            var current = Interlocked.Increment(ref concurrent);
            InterlockedMax(ref maxConcurrent, current);

            await Task.Delay(20);

            Interlocked.Decrement(ref concurrent);
            return new WeaponAggregate { ItemId = itemId, VehicleId = 0 };
        });

        var sut = new WeaponAggregateService(repository.Object, TestCache.Create());

        var result = await sut.GetAggregates(Enumerable.Range(1, 60));

        Assert.Equal(60, result.Count);
        Assert.True(maxConcurrent <= 8, $"Expected at most 8 concurrent lookups but saw {maxConcurrent}");
    }

    [Fact]
    public async Task GetAggregates_LimitsConcurrentDatabaseLookups_AcrossSimultaneousCalls()
    {
        // The service is a singleton shared by every request, so the limit has to hold across calls, not just within one.
        var concurrent = 0;
        var maxConcurrent = 0;

        var repository = new Mock<IWeaponAggregateRepository>();
        repository.Setup(a => a.GetWeaponAggregateByItemId(It.IsAny<int>())).Returns(async (int itemId) =>
        {
            var current = Interlocked.Increment(ref concurrent);
            InterlockedMax(ref maxConcurrent, current);

            await Task.Delay(20);

            Interlocked.Decrement(ref concurrent);
            return new WeaponAggregate { ItemId = itemId, VehicleId = 0 };
        });

        var sut = new WeaponAggregateService(repository.Object, TestCache.Create());

        var calls = Enumerable.Range(0, 10).Select(call => sut.GetAggregates(Enumerable.Range(call * 20, 20)));
        var results = await Task.WhenAll(calls);

        Assert.All(results, result => Assert.Equal(20, result.Count));
        Assert.True(maxConcurrent <= 8, $"Expected at most 8 concurrent lookups but saw {maxConcurrent}");
    }

    [Fact]
    public async Task GetAggregates_SkipsItemsWithoutAnAggregate_AndKeysByItemAndVehicle()
    {
        var repository = new Mock<IWeaponAggregateRepository>();
        repository.Setup(a => a.GetWeaponAggregateByItemId(1)).ReturnsAsync(new WeaponAggregate { ItemId = 1, VehicleId = 5 });
        repository.Setup(a => a.GetWeaponAggregateByItemId(2)).ReturnsAsync((WeaponAggregate)null);

        var sut = new WeaponAggregateService(repository.Object, TestCache.Create());

        var result = await sut.GetAggregates([1, 2, 1]);

        var aggregate = Assert.Single(result);
        Assert.Equal("1-5", aggregate.Key);
    }

    private static void InterlockedMax(ref int location, int value)
    {
        int initial;
        do
        {
            initial = location;
            if (value <= initial)
            {
                return;
            }
        }
        while (Interlocked.CompareExchange(ref location, value, initial) != initial);
    }
}

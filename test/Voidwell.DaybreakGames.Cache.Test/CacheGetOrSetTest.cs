using FluentAssertions;
using Xunit;
using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.DaybreakGames.Cache.Test;

public class CacheGetOrSetTest
{
    private static ICache CreateCache(FusionCache fusion)
    {
        return new Voidwell.DaybreakGames.Cache.Cache(fusion, new MemoryListStore());
    }

    [Fact]
    public async Task GetOrSetAsync_RunsFactoryOnce()
    {
        using var fusion = new FusionCache(new FusionCacheOptions());
        var cache = CreateCache(fusion);
        var calls = 0;

        Task<string> Factory(CancellationToken ct)
        {
            calls++;
            return Task.FromResult("value");
        }

        var first = await cache.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), TestContext.Current.CancellationToken);
        var second = await cache.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), TestContext.Current.CancellationToken);

        first.Should().Be("value");
        second.Should().Be("value");
        calls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrSetIfNotNullAsync_DoesNotCacheNull()
    {
        using var fusion = new FusionCache(new FusionCacheOptions());
        var cache = CreateCache(fusion);
        var calls = 0;

        Task<string> Factory(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(calls < 3 ? null : "value");
        }

        var token = TestContext.Current.CancellationToken;
        (await cache.GetOrSetIfNotNullAsync("key", Factory, TimeSpan.FromMinutes(1), token)).Should().BeNull();
        (await cache.GetOrSetIfNotNullAsync("key", Factory, TimeSpan.FromMinutes(1), token)).Should().BeNull();
        (await cache.GetOrSetIfNotNullAsync("key", Factory, TimeSpan.FromMinutes(1), token)).Should().Be("value");
        (await cache.GetOrSetIfNotNullAsync("key", Factory, TimeSpan.FromMinutes(1), token)).Should().Be("value");

        calls.Should().Be(3);
    }
}

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Voidwell.DaybreakGames.Cache.Test;

public class CacheRegistrationTest
{
    [Fact]
    public async Task AddCache_WithoutRedis_UsesMemoryOnly()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCache(options => options.KeyPrefix = "test:");

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICache>();

        var value = await cache.GetOrSetAsync("key", ct => Task.FromResult("value"), TimeSpan.FromMinutes(1), TestContext.Current.CancellationToken);

        value.Should().Be("value");
        provider.GetRequiredService<IListStore>().Should().BeOfType<MemoryListStore>();
    }

    [Fact]
    public async Task AddCache_WithUnreachableRedis_StillServesFromMemory()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCache(options =>
        {
            options.KeyPrefix = "test:";
            options.RedisConfiguration = "127.0.0.1:1,abortConnect=false,connectTimeout=500,syncTimeout=500,asyncTimeout=500";
        });

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICache>();
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
        provider.GetRequiredService<IListStore>().Should().BeOfType<RedisListStore>();
    }
}

using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Voidwell.DaybreakGames.Cache.Test;

/// <summary>
/// Redis failures must never surface to callers; they degrade to "no data".
/// </summary>
public class RedisListStoreTest
{
    private static RedisListStore CreateUnreachableStore()
    {
        return new RedisListStore(Options.Create(new CacheOptions
        {
            KeyPrefix = "test",
            RedisConfiguration = "127.0.0.1:1,abortConnect=true,connectTimeout=300,connectRetry=1"
        }));
    }

    [Fact]
    public async Task AddAsync_RedisUnavailable_DoesNotThrow()
    {
        using var sut = CreateUnreachableStore();

        await sut.Invoking(s => s.AddAsync("list", "a")).Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveAsync_RedisUnavailable_DoesNotThrow()
    {
        using var sut = CreateUnreachableStore();

        await sut.Invoking(s => s.RemoveAsync("list", "a")).Should().NotThrowAsync();
    }

    [Fact]
    public async Task ClearAsync_RedisUnavailable_DoesNotThrow()
    {
        using var sut = CreateUnreachableStore();

        await sut.Invoking(s => s.ClearAsync("list")).Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetAsync_RedisUnavailable_ReturnsNull()
    {
        using var sut = CreateUnreachableStore();

        (await sut.GetAsync("list")).Should().BeNull();
    }

    [Fact]
    public async Task GetLengthAsync_RedisUnavailable_ReturnsNull()
    {
        using var sut = CreateUnreachableStore();

        (await sut.GetLengthAsync("list")).Should().BeNull();
    }
}

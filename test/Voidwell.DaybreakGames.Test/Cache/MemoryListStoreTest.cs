using FluentAssertions;
using Voidwell.DaybreakGames.Cache;
using Xunit;

namespace Voidwell.DaybreakGames.Test.Cache;

public class MemoryListStoreTest
{
    [Fact]
    public async Task AddAsync_AddsDistinctItems()
    {
        var store = new MemoryListStore();

        await store.AddAsync("players", "a");
        await store.AddAsync("players", "b");
        await store.AddAsync("players", "a");

        (await store.GetAsync("players")).Should().BeEquivalentTo(new[] { "a", "b" });
        (await store.GetLengthAsync("players")).Should().Be(2);
    }

    [Fact]
    public async Task RemoveAsync_RemovesItem()
    {
        var store = new MemoryListStore();
        await store.AddAsync("players", "a");
        await store.AddAsync("players", "b");

        await store.RemoveAsync("players", "a");

        (await store.GetAsync("players")).Should().BeEquivalentTo(new[] { "b" });
    }

    [Fact]
    public async Task GetAsync_UnknownKey_ReturnsEmpty()
    {
        var store = new MemoryListStore();

        (await store.GetAsync("missing")).Should().BeEmpty();
        (await store.GetLengthAsync("missing")).Should().Be(0);
    }
}

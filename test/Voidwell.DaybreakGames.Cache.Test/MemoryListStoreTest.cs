using FluentAssertions;
using Xunit;

namespace Voidwell.DaybreakGames.Cache.Test;

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
    public async Task ClearAsync_RemovesAllItemsForKeyOnly()
    {
        var store = new MemoryListStore();
        await store.AddAsync("players", "a");
        await store.AddAsync("other", "b");

        await store.ClearAsync("players");

        (await store.GetAsync("players")).Should().BeEmpty();
        (await store.GetAsync("other")).Should().BeEquivalentTo(new[] { "b" });
    }

    [Fact]
    public async Task ClearAsync_UnknownKey_DoesNotThrow()
    {
        await new MemoryListStore().Invoking(s => s.ClearAsync("missing")).Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetAsync_UnknownKey_ReturnsEmpty()
    {
        var store = new MemoryListStore();

        (await store.GetAsync("missing")).Should().BeEmpty();
        (await store.GetLengthAsync("missing")).Should().Be(0);
    }
}

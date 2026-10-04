using FluentAssertions;
using Moq;
using Xunit;
using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.DaybreakGames.Cache.Test;

public class CacheTest : IDisposable
{
    private readonly FusionCache _fusion = new(new FusionCacheOptions());
    private readonly Cache _sut;

    public CacheTest()
    {
        _sut = new Cache(_fusion, new MemoryListStore());
    }

    public void Dispose()
    {
        _fusion.Dispose();
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_ReturnsValue()
    {
        await _sut.SetAsync("key", "value");

        (await _sut.GetAsync<string>("key")).Should().Be("value");
    }

    [Fact]
    public async Task SetAsync_WithExpiry_ExpiresEntry()
    {
        await _sut.SetAsync("key", "value", TimeSpan.FromMilliseconds(50));

        await Task.Delay(300, TestContext.Current.CancellationToken);

        (await _sut.GetAsync<string>("key")).Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Missing_ReturnsDefault()
    {
        (await _sut.GetAsync<string>("missing")).Should().BeNull();
        (await _sut.GetAsync<int>("missing")).Should().Be(0);
    }

    [Fact]
    public async Task TryGetAsync_Hit_InvokesCallbackAndReturnsTrue()
    {
        await _sut.SetAsync("key", 42);
        var received = 0;

        var found = await _sut.TryGetAsync<int>("key", v => received = v);

        found.Should().BeTrue();
        received.Should().Be(42);
    }

    [Fact]
    public async Task TryGetAsync_Miss_DoesNotInvokeCallback()
    {
        var invoked = false;

        var found = await _sut.TryGetAsync<int>("missing", _ => invoked = true);

        found.Should().BeFalse();
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveAsync_RemovesEntry()
    {
        await _sut.SetAsync("key", "value");

        await _sut.RemoveAsync("key");

        (await _sut.GetAsync<string>("key")).Should().BeNull();
    }

    [Fact]
    public async Task GetOrSetAsync_CacheWhenFalse_ReturnsValueButDoesNotCacheIt()
    {
        var calls = 0;

        Task<List<int>> Factory(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(new List<int>());
        }

        var token = TestContext.Current.CancellationToken;
        await _sut.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), list => list.Count > 0, token);
        await _sut.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), list => list.Count > 0, token);

        calls.Should().Be(2);
    }

    [Fact]
    public async Task GetOrSetAsync_CacheWhenTrue_CachesValue()
    {
        var calls = 0;

        Task<List<int>> Factory(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(new List<int> { 1 });
        }

        var token = TestContext.Current.CancellationToken;
        await _sut.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), list => list.Count > 0, token);
        var second = await _sut.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), list => list.Count > 0, token);

        calls.Should().Be(1);
        second.Should().Equal(1);
    }

    [Fact]
    public async Task GetOrSetAsync_ConcurrentCallers_ShareOneFactoryExecution()
    {
        var calls = 0;
        var gate = new TaskCompletionSource();

        async Task<string> Factory(CancellationToken ct)
        {
            Interlocked.Increment(ref calls);
            await gate.Task;
            return "value";
        }

        var token = TestContext.Current.CancellationToken;
        var tasks = Enumerable.Range(0, 5).Select(_ => _sut.GetOrSetAsync("key", Factory, TimeSpan.FromMinutes(1), token)).ToList();
        gate.SetResult();

        (await Task.WhenAll(tasks)).Should().OnlyContain(v => v == "value");
        calls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrSetAsync_FactoryThrows_PropagatesAndDoesNotCache()
    {
        var token = TestContext.Current.CancellationToken;

        await _sut.Invoking(c => c.GetOrSetAsync<string>("key", _ => throw new InvalidOperationException("boom"), TimeSpan.FromMinutes(1), token))
            .Should().ThrowAsync<InvalidOperationException>();

        (await _sut.GetOrSetAsync("key", _ => Task.FromResult("recovered"), TimeSpan.FromMinutes(1), token)).Should().Be("recovered");
    }

    [Fact]
    public async Task ListMethods_DelegateToListStore()
    {
        var lists = new Mock<IListStore>();
        lists.Setup(a => a.GetAsync("list")).ReturnsAsync(new[] { "a", "b" });
        lists.Setup(a => a.GetLengthAsync("list")).ReturnsAsync(2L);
        var sut = new Cache(_fusion, lists.Object);

        await sut.AddToListAsync("list", "a");
        await sut.RemoveFromListAsync("list", "b");

        lists.Verify(a => a.AddAsync("list", "a"), Times.Once);
        lists.Verify(a => a.RemoveAsync("list", "b"), Times.Once);
        (await sut.GetListAsync("list")).Should().Equal("a", "b");
        (await sut.GetListLengthAsync("list")).Should().Be(2);
    }

    [Fact]
    public async Task ClearListAsync_RemovesAllItems()
    {
        await _sut.AddToListAsync("list", "a");
        await _sut.AddToListAsync("list", "b");

        await _sut.ClearListAsync("list");

        (await _sut.GetListAsync("list")).Should().BeEmpty();
        (await _sut.GetListLengthAsync("list")).Should().Be(0);
    }

    [Fact]
    public async Task RemoveAsync_DoesNotAffectLists()
    {
        await _sut.AddToListAsync("list", "a");

        await _sut.RemoveAsync("list");

        (await _sut.GetListAsync("list")).Should().Equal("a");
    }

    [Fact]
    public async Task GetListAsync_StoreUnavailable_ReturnsEmpty()
    {
        var lists = new Mock<IListStore>();
        lists.Setup(a => a.GetAsync(It.IsAny<string>())).ReturnsAsync((IReadOnlyCollection<string>)null);
        lists.Setup(a => a.GetLengthAsync(It.IsAny<string>())).ReturnsAsync((long?)null);
        var sut = new Cache(_fusion, lists.Object);

        (await sut.GetListAsync("list")).Should().BeEmpty();
        (await sut.GetListLengthAsync("list")).Should().Be(0);
    }

    [Fact]
    public async Task TryGetListAsync_StoreUnavailable_ReturnsFalse()
    {
        var lists = new Mock<IListStore>();
        lists.Setup(a => a.GetAsync(It.IsAny<string>())).ReturnsAsync((IReadOnlyCollection<string>)null);
        var sut = new Cache(_fusion, lists.Object);
        var invoked = false;

        var found = await sut.TryGetListAsync("list", _ => invoked = true);

        found.Should().BeFalse();
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task TryGetListAsync_Hit_InvokesCallback()
    {
        await _sut.AddToListAsync("list", "a");
        IEnumerable<string> received = null;

        var found = await _sut.TryGetListAsync("list", items => received = items);

        found.Should().BeTrue();
        received.Should().Equal("a");
    }

    [Fact]
    public async Task TryGetListLengthAsync_Hit_InvokesCallback()
    {
        await _sut.AddToListAsync("list", "a");
        await _sut.AddToListAsync("list", "b");
        long length = -1;

        var found = await _sut.TryGetListLengthAsync("list", l => length = l);

        found.Should().BeTrue();
        length.Should().Be(2);
    }

    [Fact]
    public async Task TryGetListLengthAsync_StoreUnavailable_ReturnsFalse()
    {
        var lists = new Mock<IListStore>();
        lists.Setup(a => a.GetLengthAsync(It.IsAny<string>())).ReturnsAsync((long?)null);
        var sut = new Cache(_fusion, lists.Object);

        (await sut.TryGetListLengthAsync("list", _ => { })).Should().BeFalse();
    }
}

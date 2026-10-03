using FluentAssertions;
using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

public class KeyedSemaphoreSlimTest
{
    [Fact]
    public async Task WaitAsync_SameKey_BlocksUntilFirstLockIsReleased()
    {
        using var semaphore = new KeyedSemaphoreSlim();
        var token = TestContext.Current.CancellationToken;

        var first = await semaphore.WaitAsync("a", token);
        var second = semaphore.WaitAsync("a", token);

        second.IsCompleted.Should().BeFalse();

        first.Dispose();

        using var secondLock = await second.WaitAsync(TimeSpan.FromSeconds(5), token);
    }

    [Fact]
    public async Task WaitAsync_DifferentKeys_DoNotBlockEachOther()
    {
        using var semaphore = new KeyedSemaphoreSlim();
        var token = TestContext.Current.CancellationToken;

        using var first = await semaphore.WaitAsync("a", token);
        var second = semaphore.WaitAsync("b", token);

        using var secondLock = await second.WaitAsync(TimeSpan.FromSeconds(5), token);
    }

    [Fact]
    public async Task WaitAsync_AfterRelease_CanBeAcquiredAgain()
    {
        using var semaphore = new KeyedSemaphoreSlim();
        var token = TestContext.Current.CancellationToken;

        (await semaphore.WaitAsync("a", token)).Dispose();
        (await semaphore.WaitAsync("a", token)).Dispose();
        using var third = await semaphore.WaitAsync("a", token);
    }

    [Fact]
    public async Task WaitAsync_SameKey_SerializesCriticalSection()
    {
        using var semaphore = new KeyedSemaphoreSlim();
        var token = TestContext.Current.CancellationToken;
        var concurrent = 0;
        var maxConcurrent = 0;

        async Task Work()
        {
            using (await semaphore.WaitAsync("a", token))
            {
                var now = Interlocked.Increment(ref concurrent);
                maxConcurrent = Math.Max(maxConcurrent, now);
                await Task.Delay(5, token);
                Interlocked.Decrement(ref concurrent);
            }
        }

        await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Work()));

        maxConcurrent.Should().Be(1);
    }

    [Fact]
    public async Task WaitAsync_CancelledToken_Throws()
    {
        using var semaphore = new KeyedSemaphoreSlim();
        var token = TestContext.Current.CancellationToken;

        using var held = await semaphore.WaitAsync("a", token);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        var waiting = semaphore.WaitAsync("a", cts.Token);

        await cts.CancelAsync();

        await waiting.Invoking(w => w).Should().ThrowAsync<OperationCanceledException>();
    }
}

using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.Census.Test;

public class CensusContextTest
{
    private readonly CensusContext _sut = new();

    [Fact]
    public void Platform_NotSet_IsNull()
    {
        Assert.Null(_sut.Platform);
    }

    [Fact]
    public void Use_SetsPlatformUntilDisposed()
    {
        using (_sut.Use(Platform.PS4US))
        {
            Assert.Equal(Platform.PS4US, _sut.Platform);
        }

        Assert.Null(_sut.Platform);
    }

    [Fact]
    public void Use_Nested_RestoresOuterPlatform()
    {
        using (_sut.Use(Platform.PS4US))
        {
            using (_sut.Use(Platform.PS4EU))
            {
                Assert.Equal(Platform.PS4EU, _sut.Platform);
            }

            Assert.Equal(Platform.PS4US, _sut.Platform);
        }
    }

    [Fact]
    public void Use_Null_KeepsCurrentPlatform()
    {
        using (_sut.Use(Platform.PS4EU))
        {
            using (_sut.Use(null))
            {
                Assert.Equal(Platform.PS4EU, _sut.Platform);
            }

            Assert.Equal(Platform.PS4EU, _sut.Platform);
        }
    }

    [Fact]
    public async Task Use_FlowsIntoAwaitedCalls()
    {
        Platform? observed;
        using (_sut.Use(Platform.PS4EU))
        {
            observed = await Task.Run(async () =>
            {
                await Task.Yield();
                return _sut.Platform;
            }, TestContext.Current.CancellationToken);
        }

        Assert.Equal(Platform.PS4EU, observed);
    }

    [Fact]
    public async Task Use_ParallelFlows_StayIsolated()
    {
        async Task<Platform?> RunAsync(Platform platform)
        {
            using (_sut.Use(platform))
            {
                await Task.Delay(20, TestContext.Current.CancellationToken);
                return _sut.Platform;
            }
        }

        var results = await Task.WhenAll(Enum.GetValues<Platform>().Select(RunAsync));

        Assert.Equal(Enum.GetValues<Platform>().Cast<Platform?>(), results);
        Assert.Null(_sut.Platform);
    }
}

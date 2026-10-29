using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.CensusStore.Platforms;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Test.Caching;
using Voidwell.DaybreakGames.Test.Census;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class PlatformResolverTest : IDisposable
{
    private readonly FakeCensusHandler _handler = new();
    private readonly Mock<IWorldStore> _worldStore = new();
    private readonly Mock<ICharacterRepository> _characterRepository = new();
    private readonly Mock<IOutfitRepository> _outfitRepository = new();
    private readonly ServiceProvider _provider;

    public PlatformResolverTest()
    {
        _worldStore.Setup(a => a.GetAllWorlds()).ReturnsAsync(new[]
        {
            new World { Id = 1, Platform = Platform.PC },
            new World { Id = 17, Platform = Platform.PC },
            new World { Id = 1000, Platform = Platform.PS4US },
            new World { Id = 2000, Platform = Platform.PS4EU }
        });

        _provider = new ServiceCollection()
            .AddFakeCensus(_handler)
            .AddSingleton(_worldStore.Object)
            .AddSingleton(_characterRepository.Object)
            .AddSingleton(_outfitRepository.Object)
            .AddSingleton(TestCache.Create())
            .AddSingleton<IPlatformResolver, PlatformResolver>()
            .BuildServiceProvider();
    }

    public void Dispose()
    {
        _provider.Dispose();
    }

    private IPlatformResolver Sut => _provider.GetRequiredService<IPlatformResolver>();
    private ICensusContext CensusContext => _provider.GetRequiredService<ICensusContext>();

    [Fact]
    public async Task GetCharacterPlatformAsync_StoredCharacter_UsesItsWorld()
    {
        _characterRepository.Setup(a => a.GetCharacterAsync(5)).ReturnsAsync(new Character { Id = 5, WorldId = 2000 });

        Assert.Equal(Platform.PS4EU, await Sut.GetCharacterPlatformAsync(5));
        Assert.Empty(_handler.Requests);
    }

    [Fact]
    public async Task GetCharacterPlatformAsync_StoredCharacter_WinsOverContextPlatform()
    {
        _characterRepository.Setup(a => a.GetCharacterAsync(5)).ReturnsAsync(new Character { Id = 5, WorldId = 1000 });

        using (CensusContext.Use(Platform.PC))
        {
            Assert.Equal(Platform.PS4US, await Sut.GetCharacterPlatformAsync(5));
        }
    }

    [Fact]
    public async Task GetCharacterPlatformAsync_UnstoredWithContextPlatform_TrustsIt()
    {
        using (CensusContext.Use(Platform.PS4US))
        {
            Assert.Equal(Platform.PS4US, await Sut.GetCharacterPlatformAsync(5));
        }

        Assert.Empty(_handler.Requests);
    }

    [Fact]
    public async Task GetCharacterPlatformAsync_Unstored_FindsAndCachesPlatformFromCensus()
    {
        _handler.Respond("ps2ps4us", "character", """{"character_id":"5","world_id":"1000"}""");

        Assert.Equal(Platform.PS4US, await Sut.GetCharacterPlatformAsync(5));
        var requestCount = _handler.Requests.Count;
        Assert.Equal(Platform.PS4US, await Sut.GetCharacterPlatformAsync(5));

        Assert.Equal(new[] { "ps2", "ps2ps4us" }, _handler.Requests.Select(a => a.Namespace));
        Assert.Equal(requestCount, _handler.Requests.Count);
    }

    [Fact]
    public async Task GetCharacterPlatformAsync_NotFoundAnywhere_ReturnsNull()
    {
        Assert.Null(await Sut.GetCharacterPlatformAsync(5));
        Assert.Equal(3, _handler.Requests.Count);
    }

    [Fact]
    public async Task GetCharacterPlatformAsync_CensusUnreachable_Throws()
    {
        _handler.FailWith = HttpStatusCode.ServiceUnavailable;

        await Assert.ThrowsAnyAsync<Exception>(() => Sut.GetCharacterPlatformAsync(5));
    }

    [Fact]
    public async Task GetOutfitPlatformAsync_StoredOutfit_UsesItsWorld()
    {
        _outfitRepository.Setup(a => a.GetOutfitAsync(9)).ReturnsAsync(new Outfit { Id = 9, WorldId = 2000 });

        Assert.Equal(Platform.PS4EU, await Sut.GetOutfitPlatformAsync(9));
        Assert.Empty(_handler.Requests);
    }

    [Fact]
    public async Task GetContextWorldIdsAsync_NoPlatformSet_ReturnsNull()
    {
        Assert.Null(await Sut.GetContextWorldIdsAsync());
    }

    [Fact]
    public async Task GetContextWorldIdsAsync_PlatformSet_ReturnsItsWorlds()
    {
        using (CensusContext.Use(Platform.PC))
        {
            Assert.Equal(new[] { 1, 17 }, await Sut.GetContextWorldIdsAsync());
        }
    }

    [Fact]
    public void GetSearchPlatforms_FollowsContext()
    {
        Assert.Equal(Enum.GetValues<Platform>(), Sut.GetSearchPlatforms());

        using (CensusContext.Use(Platform.PS4EU))
        {
            Assert.Equal(new[] { Platform.PS4EU }, Sut.GetSearchPlatforms());
        }
    }
}

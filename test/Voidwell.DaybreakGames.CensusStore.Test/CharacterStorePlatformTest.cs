using Microsoft.Extensions.DependencyInjection;
using Moq;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.CensusStore.Platforms;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Test.Caching;
using Voidwell.DaybreakGames.Test.Census;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class CharacterStorePlatformTest : IDisposable
{
    private readonly FakeCensusHandler _handler = new();
    private readonly Mock<ICharacterRepository> _characterRepository = new();
    private readonly ServiceProvider _provider;

    public CharacterStorePlatformTest()
    {
        var worldStore = new Mock<IWorldStore>();
        worldStore.Setup(a => a.GetAllWorlds()).ReturnsAsync(new[]
        {
            new World { Id = 1, Platform = Platform.PC },
            new World { Id = 1000, Platform = Platform.PS4US },
            new World { Id = 2000, Platform = Platform.PS4EU }
        });

        _provider = new ServiceCollection()
            .AddFakeCensus(_handler)
            .AddSingleton(worldStore.Object)
            .AddSingleton(_characterRepository.Object)
            .AddSingleton(Mock.Of<IOutfitRepository>())
            .AddSingleton(Mock.Of<IOutfitStore>())
            .AddSingleton(Mock.Of<ICensusToEntityMapper>())
            .AddSingleton(TestCache.Create())
            .AddSingleton<IPlatformResolver, PlatformResolver>()
            .AddSingleton<CharacterStore>()
            .BuildServiceProvider();

        _handler
            .Respond("ps2", "character", """{"character_id":"1","name":{"first":"Danb","first_lower":"danb"},"world_id":"1"}""")
            .Respond("ps2ps4us", "character", """{"character_id":"2","name":{"first":"Dan","first_lower":"dan"},"world_id":"1000"}""")
            .Respond("ps2ps4eu", "character", """{"character_id":"3","name":{"first":"Dana","first_lower":"dana"},"world_id":"2000"}""");
    }

    public void Dispose()
    {
        _provider.Dispose();
    }

    private CharacterStore Sut => _provider.GetRequiredService<CharacterStore>();

    [Fact]
    public async Task LookupCharactersByName_NoPlatformSet_MergesEveryPlatformExactMatchFirst()
    {
        var result = await Sut.LookupCharactersByName("dan", 10);

        Assert.Equal(new long[] { 2, 3, 1 }, result.Select(a => a.CharacterId));
    }

    [Fact]
    public async Task LookupCharactersByName_PlatformSet_QueriesOnlyThatPlatform()
    {
        IEnumerable<long> result;
        using (_provider.GetRequiredService<ICensusContext>().Use(Platform.PS4EU))
        {
            result = (await Sut.LookupCharactersByName("dan", 10)).Select(a => a.CharacterId);
        }

        Assert.Equal(new long[] { 3 }, result);
        Assert.All(_handler.Requests, a => Assert.Equal("ps2ps4eu", a.Namespace));
    }

    [Fact]
    public async Task LookupCharactersByName_OnePlatformFails_ReturnsTheOthers()
    {
        _handler.FailingNamespaces.Add("ps2ps4us");

        var result = await Sut.LookupCharactersByName("dan", 10);

        Assert.Equal(new long[] { 1, 3 }, result.Select(a => a.CharacterId).Order());
        _characterRepository.Verify(a => a.LookupCharactersByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task LookupCharactersByName_EveryPlatformFails_FallsBackToStoredCharacters()
    {
        _handler.FailWith = System.Net.HttpStatusCode.InternalServerError;
        _characterRepository.Setup(a => a.LookupCharactersByNameAsync("dan", 10, null))
            .ReturnsAsync(new[] { new Character { Id = 7, Name = "Dan", WorldId = 1 } });

        var result = await Sut.LookupCharactersByName("dan", 10);

        Assert.Equal(7, Assert.Single(result).CharacterId);
    }

    [Fact]
    public async Task GetCharacter_UnstoredPs4Character_IsFetchedFromItsPlatform()
    {
        _handler
            .Respond("ps2", "character")
            .Respond("ps2ps4us", "character")
            .Respond("ps2ps4eu", "character", """{"character_id":"3","name":{"first":"Dana"},"faction_id":"1","world_id":"2000","battle_rank":{"value":"10"},"certs":{"earned_points":"5"}}""");

        var character = await Sut.GetCharacter(3);

        Assert.Equal(2000, character!.WorldId);
        _characterRepository.Verify(a => a.UpsertAsync(It.Is<Character>(c => c.Id == 3 && c.WorldId == 2000)), Times.Once);

        // PC and PS4 US are only asked while finding the character's platform
        Assert.Equal(new[] { "ps2", "ps2ps4us", "ps2ps4eu", "ps2ps4eu" }, _handler.Requests.Where(a => a.Collection == "character").Select(a => a.Namespace));
    }
}

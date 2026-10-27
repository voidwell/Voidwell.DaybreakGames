using DaybreakGames.Census;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.Test.Census;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.Census.Test;

public class PlatformCensusClientTest
{
    private readonly FakeCensusHandler _handler = new();

    private ServiceProvider CreateProvider(params Platform[] enabledPlatforms)
    {
        return new ServiceCollection().AddFakeCensus(_handler, enabledPlatforms).BuildServiceProvider();
    }

    [Fact]
    public async Task CreateQuery_NoPlatformSet_UsesPc()
    {
        using var provider = CreateProvider();
        var client = provider.GetRequiredService<ICensusClient>();

        await client.CreateQuery("character").GetAsync(TestContext.Current.CancellationToken);

        Assert.Equal(("ps2", "character"), Assert.Single(_handler.Requests));
    }

    [Theory]
    [InlineData(Platform.PC, "ps2")]
    [InlineData(Platform.PS4US, "ps2ps4us")]
    [InlineData(Platform.PS4EU, "ps2ps4eu")]
    public async Task CreateQuery_PlatformSet_UsesPlatformNamespace(Platform platform, string expectedNamespace)
    {
        using var provider = CreateProvider();
        var client = provider.GetRequiredService<ICensusClient>();
        var censusContext = provider.GetRequiredService<ICensusContext>();

        using (censusContext.Use(platform))
        {
            await client.CreateQuery("character").GetAsync(TestContext.Current.CancellationToken);
        }

        Assert.Equal((expectedNamespace, "character"), Assert.Single(_handler.Requests));
    }

    [Fact]
    public async Task Collection_HeldBySingleton_FollowsCurrentPlatform()
    {
        _handler.Respond("ps2ps4eu", "character", """{"character_id":"5","name":{"first":"EuPlayer"},"world_id":"2000"}""");
        using var provider = CreateProvider();
        var collection = provider.GetRequiredService<CharacterCollection>();
        var censusContext = provider.GetRequiredService<ICensusContext>();

        var pcCharacter = await collection.GetCharacterAsync(5);
        CensusCharacterModel euCharacter;
        using (censusContext.Use(Platform.PS4EU))
        {
            euCharacter = await collection.GetCharacterAsync(5);
        }

        Assert.Null(pcCharacter);
        Assert.Equal(2000, euCharacter.WorldId);
    }

    [Fact]
    public async Task WorldCollection_ReturnsWorldsOfEveryEnabledPlatform()
    {
        _handler
            .Respond("ps2", "world", """{"world_id":"1","state":"online","name":{"en":"Osprey"}}""")
            .Respond("ps2ps4us", "world", """{"world_id":"1000","state":"online","name":{"en":"Genudine"}}""")
            .Respond("ps2ps4eu", "world", """{"world_id":"2000","state":"online","name":{"en":"Ceres"}}""");
        using var provider = CreateProvider(Platform.PC, Platform.PS4US);

        var worlds = (await provider.GetRequiredService<WorldCollection>().GetCollectionAsync()).ToList();

        Assert.Equal(2, worlds.Count);
        Assert.Equal(Platform.PC, worlds.Single(a => a.WorldId == 1).Platform);
        Assert.Equal(Platform.PS4US, worlds.Single(a => a.WorldId == 1000).Platform);
        Assert.DoesNotContain(_handler.Requests, a => a.Namespace == "ps2ps4eu");
    }

    [Fact]
    public void GetCensusPlatforms_Empty_ReturnsEveryPlatform()
    {
        var configuration = new ConfigurationBuilder().Build();

        Assert.Equal(Enum.GetValues<Platform>(), configuration.GetCensusPlatforms());
    }

    [Fact]
    public void GetCensusPlatforms_List_ParsesCaseInsensitively()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["CensusPlatforms"] = "pc, PS4EU,ps4eu" })
            .Build();

        Assert.Equal(new[] { Platform.PC, Platform.PS4EU }, configuration.GetCensusPlatforms());
    }

    [Fact]
    public void GetCensusPlatforms_UnknownPlatform_Throws()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["CensusPlatforms"] = "PC, Xbox" })
            .Build();

        Assert.Throws<InvalidOperationException>(() => configuration.GetCensusPlatforms().ToList());
    }
}

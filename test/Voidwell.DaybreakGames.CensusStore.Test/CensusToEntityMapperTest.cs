using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class CensusToEntityMapperTest
{
    private readonly CensusToEntityMapper _sut = new();

    [Fact]
    public void Map_Generic_ReturnsMappedEntities()
    {
        var models = new[]
        {
            new CensusWorldModel { WorldId = 1, Name = new MultiLanguageString { English = "Connery" } },
            new CensusWorldModel { WorldId = 17, Name = new MultiLanguageString { English = "Emerald" } }
        };

        var worlds = _sut.Map<CensusWorldModel, World>(models).ToList();

        Assert.Equal(new[] { 1, 17 }, worlds.Select(a => a.Id));
        Assert.Equal(new[] { "Connery", "Emerald" }, worlds.Select(a => a.Name));
    }

    [Fact]
    public void Map_RuntimeTypes_ReturnsListOfEntityType()
    {
        var models = new List<CensusFactionModel> { new() { FactionId = 3, Name = new MultiLanguageString { English = "Terran Republic" } } };

        var result = _sut.Map(models, typeof(CensusFactionModel), typeof(Faction));

        Assert.Equal("Terran Republic", Assert.Single(Assert.IsType<List<Faction>>(result)).Name);
    }

    [Fact]
    public void Map_UnknownTypePair_Throws()
    {
        var act = () => _sut.Map(new List<CensusWorldModel>(), typeof(CensusWorldModel), typeof(Faction));

        Assert.ThrowsAny<InvalidOperationException>(act);
    }

    [Fact]
    public void EveryRegisteredStaticStore_HasAMapFromItsCensusModelToItsEntity()
    {
        var services = new ServiceCollection();
        services.AddCensusStores(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider();
        var collections = provider.GetRequiredService<IOptions<StaticStoreUpdaterConfiguration>>().Value.Collections;

        Assert.NotEmpty(collections);

        foreach (var config in collections)
        {
            var modelType = config.CollectionType!.GetInterfaces()
                .Single(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(ICensusStaticCollection<>))
                .GetGenericArguments()[0];

            var act = () => _sut.Map(Array.CreateInstance(modelType, 0), modelType, config.EntityType!);

            Assert.True(Record.Exception(act) is null, $"{modelType.Name} -> {config.EntityType!.Name} must be mapped");
        }
    }

    [Fact]
    public void CharacterDirectiveModels_MapEveryModelUsedByTheStoreUpdaterHelper()
    {
        var pairs = new[]
        {
            (typeof(CensusCharacterDirectiveModel), typeof(CharacterDirective)),
            (typeof(CensusCharacterDirectiveTierModel), typeof(CharacterDirectiveTier)),
            (typeof(CensusCharacterDirectiveObjectiveModel), typeof(CharacterDirectiveObjective)),
            (typeof(CensusCharacterDirectiveTreeModel), typeof(CharacterDirectiveTree)),
            (typeof(CensusCharacterAchievementModel), typeof(CharacterAchievement))
        };

        foreach (var (modelType, entityType) in pairs)
        {
            var act = () => _sut.Map(Array.CreateInstance(modelType, 0), modelType, entityType);

            Assert.Null(Record.Exception(act));
        }
    }
}

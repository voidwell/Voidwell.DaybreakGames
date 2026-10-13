using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class StaticStoreUpdateBuilderTest
{
    private class AlphaCollection : ICensusStaticCollection { }
    private class BetaCollection : ICensusStaticCollection { }
    private class GammaCollection : ICensusStaticCollection { }
    private class UnregisteredCollection : ICensusStaticCollection { }

    private class AlphaEntity { }
    private class BetaEntity { }
    private class GammaEntity { }

    private static StaticStoreUpdaterConfiguration Resolve(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IOptions<StaticStoreUpdaterConfiguration>>().Value;
    }

    [Fact]
    public void Constructor_RegistersCollectionConfiguration()
    {
        var services = new ServiceCollection();

        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services, enabled: false, periodDays: 3);

        var config = Assert.Single(Resolve(services).Collections);
        Assert.Equal(typeof(AlphaCollection), config.CollectionType);
        Assert.Equal(typeof(AlphaEntity), config.EntityType);
        Assert.Equal("AlphaStore", config.StoreName);
        Assert.False(config.Enabled);
        Assert.Equal(TimeSpan.FromDays(3), config.Period);
    }

    [Fact]
    public void Constructor_Defaults_AreEnabledWithSevenDayPeriod()
    {
        var services = new ServiceCollection();

        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);

        var config = Resolve(services).Collections.Single();
        Assert.True(config.Enabled);
        Assert.Equal(TimeSpan.FromDays(7), config.Period);
    }

    [Fact]
    public void Constructor_DuplicateCollection_Throws()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<AlphaCollection, BetaEntity>(services);

        var act = () => Resolve(services);

        var ex = Assert.ThrowsAny<ArgumentException>(act);
        Assert.Matches("(?s).*conflict.*AlphaCollection.*", ex.Message);
    }

    [Fact]
    public void Constructor_DuplicateEntity_Throws()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<BetaCollection, AlphaEntity>(services);

        var act = () => Resolve(services);

        var ex = Assert.ThrowsAny<ArgumentException>(act);
        Assert.Matches("(?s).*conflict.*AlphaEntity.*", ex.Message);
    }

    [Fact]
    public void WithUpdateDependency_RegisteredDependency_IsAdded()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<BetaCollection, BetaEntity>(services)
            .WithUpdateDependency<AlphaCollection>();

        var config = Resolve(services);

        Assert.Equal(new[] { typeof(AlphaCollection) }, config.Collections.Single(a => a.CollectionType == typeof(BetaCollection)).Dependencies);
        Assert.Empty(config.Collections.Single(a => a.CollectionType == typeof(AlphaCollection)).Dependencies);
    }

    [Fact]
    public void WithUpdateDependency_MultipleDependencies_AreAllAdded()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<BetaCollection, BetaEntity>(services);
        new StaticStoreUpdateBuilder<GammaCollection, GammaEntity>(services)
            .WithUpdateDependency<AlphaCollection>()
            .WithUpdateDependency<BetaCollection>();

        var config = Resolve(services);

        Assert.Equivalent(new[] { typeof(AlphaCollection), typeof(BetaCollection) }, config.Collections.Single(a => a.CollectionType == typeof(GammaCollection)).Dependencies);
    }

    [Fact]
    public void WithUpdateDependency_OnItself_Throws()
    {
        var services = new ServiceCollection();
        var builder = new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);

        var act = () => builder.WithUpdateDependency<AlphaCollection>();

        var ex = Assert.ThrowsAny<ArgumentException>(act);
        Assert.Matches("(?s).*cannot\\ depend\\ on\\ itself.*", ex.Message);
    }

    [Fact]
    public void WithUpdateDependency_UnregisteredDependency_ThrowsWhenResolved()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services)
            .WithUpdateDependency<UnregisteredCollection>();

        var act = () => Resolve(services);

        var ex = Assert.ThrowsAny<ArgumentException>(act);
        Assert.Matches("(?s).*UnregisteredCollection.*not\\ a\\ registered.*", ex.Message);
    }

    [Fact]
    public void WithUpdateDependency_DirectCycle_Throws()
    {
        var services = new ServiceCollection();
        var alpha = new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<BetaCollection, BetaEntity>(services)
            .WithUpdateDependency<AlphaCollection>(); // Beta -> Alpha
        alpha.WithUpdateDependency<BetaCollection>(); // Alpha -> Beta closes the loop

        var act = () => Resolve(services);

        var ex = Assert.ThrowsAny<ArgumentException>(act);
        Assert.Matches("(?s).*Cyclical\\ update\\ dependency.*", ex.Message);
    }

    [Fact]
    public void WithUpdateDependency_TransitiveCycle_ReportsChain()
    {
        var services = new ServiceCollection();
        var alpha = new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        var beta = new StaticStoreUpdateBuilder<BetaCollection, BetaEntity>(services);
        var gamma = new StaticStoreUpdateBuilder<GammaCollection, GammaEntity>(services);

        beta.WithUpdateDependency<AlphaCollection>();   // Beta -> Alpha
        gamma.WithUpdateDependency<BetaCollection>();   // Gamma -> Beta
        alpha.WithUpdateDependency<GammaCollection>();  // Alpha -> Gamma closes the loop

        var act = () => Resolve(services);

        var ex = Assert.ThrowsAny<ArgumentException>(act);
        Assert.Matches("(?s).*Cyclical\\ update\\ dependency.*\\->.*", ex.Message);
    }
}

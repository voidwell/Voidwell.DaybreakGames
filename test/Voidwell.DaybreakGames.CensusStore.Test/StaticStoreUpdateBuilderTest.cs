using FluentAssertions;
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

        var config = Resolve(services).Collections.Should().ContainSingle().Subject;
        config.CollectionType.Should().Be(typeof(AlphaCollection));
        config.EntityType.Should().Be(typeof(AlphaEntity));
        config.StoreName.Should().Be("AlphaStore");
        config.Enabled.Should().BeFalse();
        config.Period.Should().Be(TimeSpan.FromDays(3));
    }

    [Fact]
    public void Constructor_Defaults_AreEnabledWithSevenDayPeriod()
    {
        var services = new ServiceCollection();

        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);

        var config = Resolve(services).Collections.Single();
        config.Enabled.Should().BeTrue();
        config.Period.Should().Be(TimeSpan.FromDays(7));
    }

    [Fact]
    public void Constructor_DuplicateCollection_Throws()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<AlphaCollection, BetaEntity>(services);

        var act = () => Resolve(services);

        act.Should().Throw<ArgumentException>().WithMessage("*conflict*AlphaCollection*");
    }

    [Fact]
    public void Constructor_DuplicateEntity_Throws()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<BetaCollection, AlphaEntity>(services);

        var act = () => Resolve(services);

        act.Should().Throw<ArgumentException>().WithMessage("*conflict*AlphaEntity*");
    }

    [Fact]
    public void WithUpdateDependency_RegisteredDependency_IsAdded()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);
        new StaticStoreUpdateBuilder<BetaCollection, BetaEntity>(services)
            .WithUpdateDependency<AlphaCollection>();

        var config = Resolve(services);

        config.Collections.Single(a => a.CollectionType == typeof(BetaCollection)).Dependencies.Should().Equal(typeof(AlphaCollection));
        config.Collections.Single(a => a.CollectionType == typeof(AlphaCollection)).Dependencies.Should().BeEmpty();
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

        config.Collections.Single(a => a.CollectionType == typeof(GammaCollection)).Dependencies
            .Should().BeEquivalentTo(new[] { typeof(AlphaCollection), typeof(BetaCollection) });
    }

    [Fact]
    public void WithUpdateDependency_OnItself_Throws()
    {
        var services = new ServiceCollection();
        var builder = new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services);

        var act = () => builder.WithUpdateDependency<AlphaCollection>();

        act.Should().Throw<ArgumentException>().WithMessage("*cannot depend on itself*");
    }

    [Fact]
    public void WithUpdateDependency_UnregisteredDependency_ThrowsWhenResolved()
    {
        var services = new ServiceCollection();
        new StaticStoreUpdateBuilder<AlphaCollection, AlphaEntity>(services)
            .WithUpdateDependency<UnregisteredCollection>();

        var act = () => Resolve(services);

        act.Should().Throw<ArgumentException>().WithMessage("*UnregisteredCollection*not a registered*");
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

        act.Should().Throw<ArgumentException>().WithMessage("*Cyclical update dependency*");
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

        act.Should().Throw<ArgumentException>().WithMessage("*Cyclical update dependency*->*");
    }
}

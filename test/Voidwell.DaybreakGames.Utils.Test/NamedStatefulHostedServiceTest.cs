using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Utils.HostedService;
using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

public class NamedStatefulHostedServiceTest
{
    private sealed class TestService : IStatefulHostedService
    {
        public TestService(string name, HostedServiceState state)
        {
            Name = name;
            State = state;
        }

        public string Name { get; }
        public HostedServiceState State { get; }
        public bool Started { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Started = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task OnApplicationStartup(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task OnApplicationShutdown(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<object> GetStatusAsync(CancellationToken cancellationToken) => Task.FromResult<object>(Name);
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<ICache>());
        services.AddStatefulServiceDependencies();
        services.AddStatefulHostedService("first", (sp, state) => new TestService("first", state));
        services.AddStatefulHostedService("second", (sp, state) => new TestService("second", state));

        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddStatefulHostedService_Named_RegistersEachInstance()
    {
        using var provider = CreateProvider();

        var states = provider.GetServices<HostedServiceState>().ToList();

        Assert.Equal(new[] { "first", "second" }, states.Select(a => a.ServiceName));
        Assert.Equal("first", ((TestService)states[0].Service).Name);
        Assert.Equal("second", ((TestService)states[1].Service).Name);
        Assert.Same(states[1], ((TestService)states[1].Service).State);
    }

    [Fact]
    public void AddStatefulHostedService_Named_RegistersHostedServiceForEachInstance()
    {
        using var provider = CreateProvider();

        var wrappers = provider.GetServices<IHostedService>().OfType<StatefulHostedServiceWrapper<TestService>>();

        Assert.Equal(2, wrappers.Count());
    }

    [Fact]
    public async Task StartServiceAsync_Named_StartsOnlyThatInstance()
    {
        using var provider = CreateProvider();
        var manager = provider.GetRequiredService<IStatefulHostedServiceManager>();

        await manager.StartServiceAsync("second", TestContext.Current.CancellationToken);

        Assert.False(provider.GetRequiredKeyedService<TestService>("first").Started);
        Assert.True(provider.GetRequiredKeyedService<TestService>("second").Started);
    }
}

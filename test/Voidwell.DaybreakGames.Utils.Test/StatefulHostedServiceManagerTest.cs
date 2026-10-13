using Microsoft.Extensions.DependencyInjection;
using Moq;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Utils.HostedService;
using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

public class StatefulHostedServiceManagerTest
{
    private readonly Mock<IStatefulHostedService> _service = new();
    private readonly Mock<ICache> _cache = new();

    private StatefulHostedServiceManager CreateSut()
    {
        _service.Setup(a => a.GetStatusAsync(It.IsAny<CancellationToken>())).ReturnsAsync("details");
        _service.Setup(a => a.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _service.Setup(a => a.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(_service.Object);
        services.AddSingleton<HostedServiceState>(sp =>
        {
            var state = new HostedServiceState<IStatefulHostedService>(sp);
            state.SetServiceName("test-service");
            return state;
        });

        return new StatefulHostedServiceManager(services.BuildServiceProvider(), _cache.Object);
    }

    [Fact]
    public void VerifyServiceExists_KnownService_ReturnsTrue()
    {
        Assert.True(CreateSut().VerifyServiceExists("test-service"));
    }

    [Fact]
    public void VerifyServiceExists_UnknownService_ReturnsFalse()
    {
        Assert.False(CreateSut().VerifyServiceExists("other"));
    }

    [Fact]
    public async Task GetServiceStatusAsync_UnknownService_ReturnsNull()
    {
        var result = await CreateSut().GetServiceStatusAsync("other", TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetServiceStatusAsync_KnownService_ReportsStateAndDetails()
    {
        var result = await CreateSut().GetServiceStatusAsync("test-service", TestContext.Current.CancellationToken);

        Assert.Equal("test-service", result.Name);
        Assert.False(result.IsEnabled);
        Assert.Equal("details", result.Details);
    }

    [Fact]
    public async Task StartServiceAsync_StartsServiceAndPersistsEnabledState()
    {
        var sut = CreateSut();

        await sut.StartServiceAsync("test-service", TestContext.Current.CancellationToken);

        _service.Verify(a => a.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(a => a.SetAsync("service-test-service-state", It.Is<object>(o => ((ServiceState)o).IsEnabled)), Times.Once);
        Assert.True((await sut.GetServiceStatusAsync("test-service", TestContext.Current.CancellationToken)).IsEnabled);
    }

    [Fact]
    public async Task StopServiceAsync_StopsServiceAndPersistsDisabledState()
    {
        var sut = CreateSut();
        await sut.StartServiceAsync("test-service", TestContext.Current.CancellationToken);

        await sut.StopServiceAsync("test-service", TestContext.Current.CancellationToken);

        _service.Verify(a => a.StopAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(a => a.SetAsync("service-test-service-state", It.Is<object>(o => !((ServiceState)o).IsEnabled)), Times.Once);
    }

    [Fact]
    public async Task StartServiceAsync_UnknownService_DoesNothing()
    {
        var sut = CreateSut();

        await sut.StartServiceAsync("other", TestContext.Current.CancellationToken);

        _service.Verify(a => a.StartAsync(It.IsAny<CancellationToken>()), Times.Never);
        _cache.Verify(a => a.SetAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task StartAsync_ServiceWasEnabledInCache_RestartsIt()
    {
        var sut = CreateSut();
        _cache.Setup(a => a.GetAsync<ServiceState>("service-test-service-state")).ReturnsAsync(new ServiceState { IsEnabled = true });

        await sut.StartAsync(TestContext.Current.CancellationToken);

        _service.Verify(a => a.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True((await sut.GetServiceStatusAsync("test-service", TestContext.Current.CancellationToken)).IsEnabled);
    }

    [Fact]
    public async Task StartAsync_ServiceWasDisabledInCache_DoesNotStartIt()
    {
        var sut = CreateSut();
        _cache.Setup(a => a.GetAsync<ServiceState>("service-test-service-state")).ReturnsAsync(new ServiceState { IsEnabled = false });

        await sut.StartAsync(TestContext.Current.CancellationToken);

        _service.Verify(a => a.StartAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartAsync_NothingInCache_DoesNotStartService()
    {
        var sut = CreateSut();

        await sut.StartAsync(TestContext.Current.CancellationToken);

        _service.Verify(a => a.StartAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

using Moq;
using Voidwell.DaybreakGames.Utils.HostedService;
using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

public class StatefulHostedServiceWrapperTest
{
    [Fact]
    public async Task StartAsync_CallsOnApplicationStartup()
    {
        var service = new Mock<IStatefulHostedService>();
        service.Setup(a => a.OnApplicationStartup(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var sut = new StatefulHostedServiceWrapper<IStatefulHostedService>(service.Object);

        await sut.StartAsync(TestContext.Current.CancellationToken);

        service.Verify(a => a.OnApplicationStartup(TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task StopAsync_CallsOnApplicationShutdown()
    {
        var service = new Mock<IStatefulHostedService>();
        service.Setup(a => a.OnApplicationShutdown(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var sut = new StatefulHostedServiceWrapper<IStatefulHostedService>(service.Object);

        await sut.StopAsync(TestContext.Current.CancellationToken);

        service.Verify(a => a.OnApplicationShutdown(TestContext.Current.CancellationToken), Times.Once);
    }
}

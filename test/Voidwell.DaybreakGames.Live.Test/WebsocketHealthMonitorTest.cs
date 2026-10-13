using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class WebsocketHealthMonitorTest : IClassFixture<WebsocketHealthMonitorFixture>
{
    private readonly WebsocketHealthMonitorFixture _fixture;

    public WebsocketHealthMonitorTest(WebsocketHealthMonitorFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetFixture();
    }

    [Fact]
    public void IsHealthy_NoEvents_True()
    {
        var sut = _fixture.CreateSut();

        var result = sut.IsHealthy();

        Assert.True(result);
    }

    [Fact]
    public void IsHealthy_RecentEvents_True()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(1, "Death", DateTime.UtcNow.AddMinutes(-2));

        var result = sut.IsHealthy();

        Assert.True(result);
    }

    [Fact]
    public void IsHealthy_StagnantEvents_False()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(1, "Death", DateTime.UtcNow.AddMinutes(-6));

        var result = sut.IsHealthy();

        Assert.False(result);
    }

    [Fact]
    public void IsHealthy_ClearedWorld_True()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(1, "Death", DateTime.UtcNow.AddMinutes(-6));

        sut.ClearWorld(1);

        var result = sut.IsHealthy();

        Assert.True(result);
    }
}

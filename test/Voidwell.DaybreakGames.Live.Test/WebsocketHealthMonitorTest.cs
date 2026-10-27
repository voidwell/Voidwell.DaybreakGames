using Voidwell.DaybreakGames.Utils;
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

        var result = sut.IsHealthy(Platform.PC);

        Assert.True(result);
    }

    [Fact]
    public void IsHealthy_RecentEvents_True()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(Platform.PC, 1, "Death", DateTime.UtcNow.AddMinutes(-2));

        var result = sut.IsHealthy(Platform.PC);

        Assert.True(result);
    }

    [Fact]
    public void IsHealthy_StagnantEvents_False()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(Platform.PC, 1, "Death", DateTime.UtcNow.AddMinutes(-6));

        var result = sut.IsHealthy(Platform.PC);

        Assert.False(result);
    }

    [Fact]
    public void IsHealthy_ClearedWorld_True()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(Platform.PC, 1, "Death", DateTime.UtcNow.AddMinutes(-6));

        sut.ClearWorld(1);

        var result = sut.IsHealthy(Platform.PC);

        Assert.True(result);
    }

    [Fact]
    public void IsHealthy_StagnantEventsOnOtherPlatform_True()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(Platform.PS4US, 1000, "Death", DateTime.UtcNow.AddMinutes(-6));
        sut.ReceivedEvent(Platform.PC, 1, "Death", DateTime.UtcNow.AddMinutes(-2));

        Assert.True(sut.IsHealthy(Platform.PC));
        Assert.False(sut.IsHealthy(Platform.PS4US));
    }

    [Fact]
    public void ClearAllWorlds_OnlyClearsThatPlatform()
    {
        var sut = _fixture.CreateSut();

        sut.ReceivedEvent(Platform.PC, 1, "Death", DateTime.UtcNow.AddMinutes(-6));
        sut.ReceivedEvent(Platform.PS4EU, 2000, "Death", DateTime.UtcNow.AddMinutes(-6));

        sut.ClearAllWorlds(Platform.PS4EU);

        Assert.True(sut.IsHealthy(Platform.PS4EU));
        Assert.False(sut.IsHealthy(Platform.PC));
    }
}

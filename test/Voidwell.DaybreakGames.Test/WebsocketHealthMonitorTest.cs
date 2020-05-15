using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Voidwell.DaybreakGames.Test
{
    public class WebsocketHealthMonitorTest : IClassFixture<WebsocketHealthMonitorFixture>
    {
        private readonly WebsocketHealthMonitorFixture _fixture;

        public WebsocketHealthMonitorTest(WebsocketHealthMonitorFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetFixture();
        }

        [Fact]
        public async Task IsHealthy_NoEvents_True()
        {
            var sut = _fixture.CreateSut();

            var result = sut.IsHealthy();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthy_RecentEvents_True()
        {
            var sut = _fixture.CreateSut();

            sut.ReceivedEvent(1, "Death", DateTime.UtcNow.AddMinutes(-2));

            var result = sut.IsHealthy();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthy_StagnantEvents_False()
        {
            var sut = _fixture.CreateSut();

            sut.ReceivedEvent(1, "Death", DateTime.UtcNow.AddMinutes(-6));

            var result = sut.IsHealthy();

            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsHealthy_ClearedWorld_True()
        {
            var sut = _fixture.CreateSut();

            sut.ReceivedEvent(1, "Death", DateTime.UtcNow.AddMinutes(-6));

            sut.ClearWorld(1);

            var result = sut.IsHealthy();

            result.Should().BeTrue();
        }
    }
}

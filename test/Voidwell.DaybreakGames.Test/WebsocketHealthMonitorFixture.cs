using Microsoft.Extensions.Logging;
using Moq;
using Voidwell.DaybreakGames.Live.CensusStream;

namespace Voidwell.DaybreakGames.Test
{
    public class WebsocketHealthMonitorFixture
    {
        private ILogger<WebsocketHealthMonitor> Logger { get; set; }

        public WebsocketHealthMonitor CreateSut()
        {
            return new WebsocketHealthMonitor(Logger);
        }

        public void ResetFixture()
        {
            Logger = Mock.Of<ILogger<WebsocketHealthMonitor>>();
        }
    }
}

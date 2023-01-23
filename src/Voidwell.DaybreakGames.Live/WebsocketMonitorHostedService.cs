using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.Live
{
    public class WebsocketMonitorHostedService : StatefulHostedServiceClient
    {
        public WebsocketMonitorHostedService(IWebsocketMonitor websocketMonitor) : base(websocketMonitor)
        {
        }
    }
}

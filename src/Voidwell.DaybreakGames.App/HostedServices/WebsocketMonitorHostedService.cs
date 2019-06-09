using Voidwell.DaybreakGames.CensusStream;

namespace Voidwell.DaybreakGames.HostedServices
{
    public class WebsocketMonitorHostedService : StatefulHostedServiceClient
    {
        public WebsocketMonitorHostedService(IWebsocketMonitor websocketMonitor) : base(websocketMonitor)
        {
        }
    }
}

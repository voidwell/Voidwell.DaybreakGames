using Voidwell.DaybreakGames.CensusStream;

namespace Voidwell.DaybreakGames.App.HostedServices
{
    public class WebsocketMonitorHostedService : StatefulHostedServiceClient
    {
        public WebsocketMonitorHostedService(IWebsocketMonitor websocketMonitor) : base(websocketMonitor)
        {
        }
    }
}

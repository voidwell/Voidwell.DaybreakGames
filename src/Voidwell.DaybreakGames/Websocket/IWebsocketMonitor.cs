using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Websocket
{
    public interface IWebsocketMonitor
    {
        Task StartAsync();
        Task StopAsync();
        bool IsRunning();
    }
}

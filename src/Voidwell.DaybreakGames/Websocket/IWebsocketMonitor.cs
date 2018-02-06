using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Websocket
{
    public interface IWebsocketMonitor
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        bool IsRunning();
    }
}

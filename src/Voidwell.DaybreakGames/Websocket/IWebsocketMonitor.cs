using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Websocket
{
    public interface IWebsocketMonitor
    {
        string GetStatus();
        Task StartMonitor();
        Task StopMonitor();
        Task ResetMonitor();
    }
}
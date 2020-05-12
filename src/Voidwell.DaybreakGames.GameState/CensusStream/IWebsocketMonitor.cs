using System.Threading.Tasks;
using Voidwell.DaybreakGames.GameState.CensusStream.Models;

namespace Voidwell.DaybreakGames.GameState.CensusStream
{
    public interface IWebsocketMonitor
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        CensusHeartbeat GetLastHeartbeat();
    }
}

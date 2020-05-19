using DaybreakGames.Census.Stream;
using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IStreamClient : IDisposable
    {
        StreamClient OnDisconnect(Func<DisconnectionInfo, Task> onDisconnect);
        StreamClient OnMessage(Func<string, Task> onMessage);
        Task ConnectAsync(CensusStreamSubscription subscription);
        Task DisconnectAsync();
        Task ReconnectAsync();
    }
}
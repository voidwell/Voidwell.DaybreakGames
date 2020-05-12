using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.GameState.CensusStream
{
    public interface IWebsocketEventHandler : IDisposable
    {
        Task Process(JToken jPayload);
    }
}
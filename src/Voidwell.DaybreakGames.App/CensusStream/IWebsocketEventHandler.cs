using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IWebsocketEventHandler : IDisposable
    {
        Task Process(JToken jPayload);
    }
}
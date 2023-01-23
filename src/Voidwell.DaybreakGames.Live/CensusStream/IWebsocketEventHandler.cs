using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public interface IWebsocketEventHandler
    {
        Task Process(JToken jPayload);
    }
}
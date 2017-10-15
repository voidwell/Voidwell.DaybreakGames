using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Websocket
{
    public interface IWebsocketEventHandler
    {
        Task Process(JToken jPayload);
    }
}
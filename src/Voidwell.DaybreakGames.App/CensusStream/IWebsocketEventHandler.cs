using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IWebsocketEventHandler
    {
        Task Process(JToken jPayload);
    }
}
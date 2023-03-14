using System.Text.Json;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public interface IWebsocketEventHandler
    {
        Task Process(JsonElement jPayload);
    }
}
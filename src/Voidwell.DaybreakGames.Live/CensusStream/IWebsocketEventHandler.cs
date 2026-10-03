using System.Text.Json;

namespace Voidwell.DaybreakGames.Live.CensusStream;

public interface IWebsocketEventHandler
{
    Task Process(JsonElement jPayload);
}

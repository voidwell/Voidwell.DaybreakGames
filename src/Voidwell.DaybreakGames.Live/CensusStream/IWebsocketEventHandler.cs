using System.Text.Json;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Live.CensusStream;

public interface IWebsocketEventHandler
{
    Task Process(Platform platform, JsonElement jPayload);
}

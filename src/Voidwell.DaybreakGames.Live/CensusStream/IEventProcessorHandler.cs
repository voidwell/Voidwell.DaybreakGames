
using System.Text.Json;

namespace Voidwell.DaybreakGames.Live.CensusStream;

public interface IEventProcessorHandler
{
    Task<bool> TryProcessAsync(string eventName, JsonElement payload);
}

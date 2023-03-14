using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public interface IEventProcessorHandler
    {
        Task<bool> TryProcessAsync(string eventName, JsonElement payload);
    }
}
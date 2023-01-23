using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public interface IEventProcessorHandler
    {
        Task<bool> TryProcessAsync(string eventName, JToken payload);
    }
}
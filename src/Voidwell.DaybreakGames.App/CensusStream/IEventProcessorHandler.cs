using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IEventProcessorHandler
    {
        Task<bool> TryProcessAsync(string eventName, JToken payload);
    }
}
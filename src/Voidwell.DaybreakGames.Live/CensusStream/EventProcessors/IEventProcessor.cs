using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    public interface IEventProcessor<TPayload> where TPayload: class
    {
        Task Process(TPayload payload);
    }
}

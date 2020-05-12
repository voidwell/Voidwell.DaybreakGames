using System.Threading.Tasks;
using Voidwell.DaybreakGames.GameState.CensusStream.Models;

namespace Voidwell.DaybreakGames.GameState.Services
{
    public interface IMetagameEventMonitor
    {
        Task ProcessMetagameEvent(MetagameEvent metagameEvent);
    }
}
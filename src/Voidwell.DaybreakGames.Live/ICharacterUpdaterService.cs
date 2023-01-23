using System.Threading.Tasks;
using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.Live
{
    public interface ICharacterUpdaterService : IStatefulHostedService
    {
        Task AddToQueue(string characterId);
    }
}

using System.Threading.Tasks;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ICharacterUpdaterService : IStatefulHostedService
    {
        Task AddToQueue(string characterId);
    }
}

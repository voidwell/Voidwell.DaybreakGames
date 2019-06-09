using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ICharacterUpdaterService : IStatefulHostedService
    {
        Task AddToQueue(string characterId);
    }
}

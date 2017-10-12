using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IUpdaterService
    {
        Task AddToQueue(string characterId);
        void StartUpdater();
        void StopUpdater();
    }
}

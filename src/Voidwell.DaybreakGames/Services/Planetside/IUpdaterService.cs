using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IUpdaterService
    {
        Task AddToQueue(string characterId);
        Task StartUpdater();
        Task Startup();
        Task StopUpdater();
    }
}

using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IUpdaterSchedulerRepository
    {
        UpdaterScheduler GetUpdaterHistoryByServiceName(string serviceName);
        Task UpsertAsync(UpdaterScheduler entity);
    }
}

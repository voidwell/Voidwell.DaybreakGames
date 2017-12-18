using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IUpdaterSchedulerRepository
    {
        DbPS2UpdaterScheduler GetUpdaterHistoryByServiceName(string serviceName);
        Task UpsertAsync(DbPS2UpdaterScheduler entity);
    }
}

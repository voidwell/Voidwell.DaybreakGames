using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IAlertRepository
    {
        Task<Alert> GetActiveAlert(int worldId, int zoneId);
        Task UpdateAsync(Alert entity);
        Task<IEnumerable<Alert>> GetAllAlerts(int limit);
        Task<IEnumerable<Alert>> GetAlertsByWorldId(int worldId, int limit);
        Task AddAsync(Alert dataModel);
        Task<Alert> GetAlert(int worldId, int instanceId);
        Task<int?> GetMetagameCategoryZoneId(int metagameEventId);
    }
}
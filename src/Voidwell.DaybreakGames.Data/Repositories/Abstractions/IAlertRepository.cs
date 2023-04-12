using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IAlertRepository
    {
        Task<Alert> GetActiveAlert(int worldId, int zoneId);
        Task<IEnumerable<Alert>> GetActiveAlertsByWorldId(int worldId);
        Task<IEnumerable<Alert>> GetAlerts(int pageNumber, int limit, int? worldId);
        Task UpdateAsync(Alert entity);
        Task AddAsync(Alert dataModel);
        Task<Alert> GetAlert(int worldId, int instanceId);
    }
}
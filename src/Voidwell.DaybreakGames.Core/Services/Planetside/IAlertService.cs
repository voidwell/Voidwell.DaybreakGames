using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetAlerts(int pageNumber, int? worldId = null, int limit = 10);
        Task<Alert> GetAlert(int worldId, int instanceId);
        Task<AlertResult> GetAlertStats(int worldId, int instanceId);
        Task<IEnumerable<Alert>> GetActiveAlertsByWorldId(int worldId);
        Task CreateAlert(Alert alertModel);
        Task UpdateAlert(Alert alertModel);
        Task<Alert> GetActiveAlert(int worldId, int zoneId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IAlertRepository
    {
        Task<DbAlert> GetActiveAlert(string worldId, string zoneId);
        Task UpdateAsync(DbAlert entity);
        Task<IEnumerable<DbAlert>> GetAllAlerts(int limit);
        Task<IEnumerable<DbAlert>> GetAlertsByWorldId(string worldId, int limit);
        Task AddAsync(DbAlert dataModel);
        Task<DbAlert> GetAlert(string worldId, string instanceId);
    }
}
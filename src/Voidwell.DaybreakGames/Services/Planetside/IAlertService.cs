using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetAllAlerts(int limit = 25);
        Task<IEnumerable<Alert>> GetAlerts(int worldId, int limit = 25);
        Task<AlertResult> GetAlert(int worldId, int instanceId);
        Task CreateAlert(MetagameEvent metagameEvent);
        Task UpdateAlert(MetagameEvent metagameEvent);
    }
}
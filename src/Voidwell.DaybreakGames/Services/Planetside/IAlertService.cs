using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetAlerts(int pageNumber, int? worldId = null, int limit = 10);
        Task<AlertResult> GetAlert(int worldId, int instanceId);
        Task CreateAlert(MetagameEvent metagameEvent);
        Task UpdateAlert(MetagameEvent metagameEvent);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IVehicleService
    {
        Task RefreshStore();
        Task<IEnumerable<VehicleInfo>> GetAllVehicles();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IVehicleService : IUpdateable
    {
        Task<IEnumerable<VehicleInfo>> GetAllVehicles();
    }
}

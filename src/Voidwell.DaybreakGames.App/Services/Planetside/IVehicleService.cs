using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IVehicleService : IUpdateable
    {
        Task<IEnumerable<VehicleInfo>> GetAllVehicles();
    }
}

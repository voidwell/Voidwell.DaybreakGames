using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task UpsertRangeAsync(IEnumerable<Vehicle> entities);
        Task UpsertRangeAsync(IEnumerable<VehicleFaction> entities);
    }
}

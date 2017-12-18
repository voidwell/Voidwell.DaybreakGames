using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<DbVehicle>> GetAllVehiclesAsync();
        Task UpsertRangeAsync(IEnumerable<DbVehicle> entities);
        Task UpsertRangeAsync(IEnumerable<DbVehicleFaction> entities);
    }
}

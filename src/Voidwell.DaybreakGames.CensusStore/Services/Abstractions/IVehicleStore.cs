using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IVehicleStore
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
    }
}
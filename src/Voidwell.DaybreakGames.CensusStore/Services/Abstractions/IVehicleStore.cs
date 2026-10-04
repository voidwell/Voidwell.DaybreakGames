using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface IVehicleStore
{
    Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
}

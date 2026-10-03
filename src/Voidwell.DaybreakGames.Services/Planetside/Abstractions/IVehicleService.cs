using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface IVehicleService
{
    Task<IEnumerable<VehicleInfo>> GetAllVehicles();
}

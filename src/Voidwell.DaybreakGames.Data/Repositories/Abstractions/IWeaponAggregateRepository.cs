using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions;

public interface IWeaponAggregateRepository
{
    Task<IEnumerable<WeaponAggregate>> GetWeaponAggregatesByItemIds(params int[] itemIds);
    Task<WeaponAggregate?> GetWeaponAggregateByVehicleId(int vehicleId);
}

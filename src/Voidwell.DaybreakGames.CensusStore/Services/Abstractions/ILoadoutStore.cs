using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface ILoadoutStore
{
    Task<IEnumerable<Loadout>> GetAllLoadoutsAsync();
}

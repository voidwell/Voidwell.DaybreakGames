using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface IWorldStore
{
    Task<IEnumerable<World>?> GetAllWorlds();
    Task<IEnumerable<DailyPopulation>?> GetWorldPopulationHistory(int worldId, DateTime start, DateTime end);
}

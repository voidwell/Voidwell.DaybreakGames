using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater;

/// <summary>
/// Upserts the worlds census returns and marks any world it no longer returns as inactive.
/// </summary>
public class WorldStoreUpdateHandler : IStoreUpdateHandler<World>
{
    private readonly IWorldRepository _worldRepository;

    public WorldStoreUpdateHandler(IWorldRepository worldRepository)
    {
        _worldRepository = worldRepository;
    }

    public Task UpdateAsync(IEnumerable<World> entities)
    {
        return _worldRepository.SyncWorldsAsync(entities);
    }
}

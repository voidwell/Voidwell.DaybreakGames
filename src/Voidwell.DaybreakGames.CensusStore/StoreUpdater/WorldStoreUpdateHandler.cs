using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater;

/// <summary>
/// Upserts the worlds census returns and marks any world it no longer returns as inactive.
/// Only platforms census returned worlds for are checked, so a platform that failed or is disabled keeps its worlds.
/// </summary>
public class WorldStoreUpdateHandler : IStoreUpdateHandler<World>
{
    private readonly IWorldRepository _worldRepository;

    public WorldStoreUpdateHandler(IWorldRepository worldRepository)
    {
        _worldRepository = worldRepository;
    }

    public async Task UpdateAsync(IEnumerable<World> entities)
    {
        var censusWorlds = entities.ToList();
        if (!censusWorlds.Any())
        {
            // An empty response is more likely a census problem than every world being retired.
            return;
        }

        var storedWorlds = (await _worldRepository.GetAllWorldsAsync()).ToDictionary(a => a.Id);
        var censusWorldIds = censusWorlds.Select(a => a.Id).ToHashSet();
        var censusPlatforms = censusWorlds.Select(a => a.Platform).ToHashSet();

        // New worlds are active; existing worlds keep their state so a world deactivated by hand stays inactive.
        foreach (var world in censusWorlds)
        {
            world.IsActive = !storedWorlds.TryGetValue(world.Id, out var storedWorld) || storedWorld.IsActive;
        }

        var missingWorlds = storedWorlds.Values
            .Where(a => a.IsActive && censusPlatforms.Contains(a.Platform) && !censusWorldIds.Contains(a.Id))
            .ToList();
        missingWorlds.ForEach(a => a.IsActive = false);

        await _worldRepository.UpsertRangeAsync(censusWorlds.Concat(missingWorlds));
    }
}

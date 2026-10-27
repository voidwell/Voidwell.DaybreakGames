using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.CensusStore.Platforms;

/// <summary>
/// Works out which platform an entity belongs to, so its Census calls go to that platform's namespace.
/// </summary>
public interface IPlatformResolver
{
    /// <summary>
    /// The platform of a stored world, or null for a world that isn't stored yet.
    /// </summary>
    Task<Platform?> GetWorldPlatformAsync(int worldId);

    /// <summary>
    /// The ids of the stored worlds on the platform set for the current flow, or null when no platform is set.
    /// </summary>
    Task<IEnumerable<int>?> GetContextWorldIdsAsync();

    /// <summary>
    /// Resolves from the stored character's world, then the platform set for the current flow, then by asking each enabled platform.
    /// </summary>
    Task<Platform?> GetCharacterPlatformAsync(long characterId);

    /// <summary>
    /// Resolves from the stored outfit's world, then the platform set for the current flow, then by asking each enabled platform.
    /// </summary>
    Task<Platform?> GetOutfitPlatformAsync(long outfitId);

    /// <summary>
    /// The platform set for the current flow, or every enabled platform when none is set.
    /// </summary>
    IEnumerable<Platform> GetSearchPlatforms();
}

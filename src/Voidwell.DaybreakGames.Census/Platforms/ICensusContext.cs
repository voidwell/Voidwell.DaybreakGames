using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Platforms;

/// <summary>
/// The platform Census calls are made against for the current async flow.
/// </summary>
public interface ICensusContext
{
    /// <summary>
    /// The platform set for the current async flow, or null when none was set.
    /// </summary>
    Platform? Platform { get; }

    /// <summary>
    /// Sets the platform for the current async flow until the returned scope is disposed.
    /// A null platform leaves the current platform unchanged.
    /// </summary>
    /// <remarks>
    /// Call this from the method that makes the Census calls, never from inside an async helper:
    /// a value set inside an awaited async method does not flow back to its caller.
    /// </remarks>
    IDisposable Use(Platform? platform);
}

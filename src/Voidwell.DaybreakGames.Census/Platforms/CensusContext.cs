using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Platforms;

public sealed class CensusContext : ICensusContext
{
    private readonly AsyncLocal<Platform?> _platform = new();

    public Platform? Platform => _platform.Value;

    public IDisposable Use(Platform? platform)
    {
        if (platform == null)
        {
            return PlatformScope.Empty;
        }

        var previous = _platform.Value;
        _platform.Value = platform;

        return new PlatformScope(() => _platform.Value = previous);
    }

    private sealed class PlatformScope : IDisposable
    {
        public static readonly PlatformScope Empty = new(null);

        private Action? _restore;

        public PlatformScope(Action? restore)
        {
            _restore = restore;
        }

        public void Dispose()
        {
            _restore?.Invoke();
            _restore = null;
        }
    }
}

using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Options;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.CensusStore.Platforms;

public class PlatformResolver : IPlatformResolver
{
    private readonly IWorldStore _worldStore;
    private readonly ICharacterRepository _characterRepository;
    private readonly IOutfitRepository _outfitRepository;
    private readonly CharacterCollection _characterCollection;
    private readonly OutfitCollection _outfitCollection;
    private readonly ICensusContext _censusContext;
    private readonly CensusPlatformOptions _options;
    private readonly ICache _cache;

    private const string _cacheKey = "ps2.platformresolver";
    private readonly Func<long, string> _getCharacterCacheKey = characterId => $"{_cacheKey}_character_{characterId}";
    private readonly Func<long, string> _getOutfitCacheKey = outfitId => $"{_cacheKey}_outfit_{outfitId}";

    // An entity never changes platform
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

    public PlatformResolver(IWorldStore worldStore, ICharacterRepository characterRepository, IOutfitRepository outfitRepository,
        CharacterCollection characterCollection, OutfitCollection outfitCollection, ICensusContext censusContext,
        IOptions<CensusPlatformOptions> options, ICache cache)
    {
        _worldStore = worldStore;
        _characterRepository = characterRepository;
        _outfitRepository = outfitRepository;
        _characterCollection = characterCollection;
        _outfitCollection = outfitCollection;
        _censusContext = censusContext;
        _options = options.Value;
        _cache = cache;
    }

    public async Task<Platform?> GetWorldPlatformAsync(int worldId)
    {
        var worlds = await _worldStore.GetAllWorlds();
        return worlds?.FirstOrDefault(a => a.Id == worldId)?.Platform;
    }

    public async Task<IEnumerable<int>?> GetContextWorldIdsAsync()
    {
        var platform = _censusContext.Platform;
        if (platform == null)
        {
            return null;
        }

        var worlds = await _worldStore.GetAllWorlds();
        return worlds?.Where(a => a.Platform == platform).Select(a => a.Id).ToList() ?? [];
    }

    public Task<Platform?> GetCharacterPlatformAsync(long characterId)
    {
        return ResolvePlatformAsync(
            _getCharacterCacheKey(characterId),
            async () =>
            {
                var character = await _characterRepository.GetCharacterAsync(characterId);
                return character != null ? await GetWorldPlatformAsync(character.WorldId) : null;
            },
            () => _characterCollection.GetCharacterAsync(characterId));
    }

    public Task<Platform?> GetOutfitPlatformAsync(long outfitId)
    {
        return ResolvePlatformAsync(
            _getOutfitCacheKey(outfitId),
            async () =>
            {
                var outfit = await _outfitRepository.GetOutfitAsync(outfitId);
                return outfit?.WorldId != null ? await GetWorldPlatformAsync(outfit.WorldId.Value) : null;
            },
            () => _outfitCollection.GetOutfitAsync(outfitId));
    }

    public IEnumerable<Platform> GetSearchPlatforms()
    {
        var platform = _censusContext.Platform;
        return platform != null ? [platform.Value] : _options.Platforms;
    }

    private async Task<Platform?> ResolvePlatformAsync<T>(string cacheKey, Func<Task<Platform?>> getStoredPlatform, Func<Task<T?>> censusQuery)
        where T : class
    {
        var platform = await _cache.GetOrSetIfNotNullAsync(cacheKey, ct => getStoredPlatform(), _cacheExpiration);
        if (platform != null)
        {
            return platform;
        }

        // Trusted but not cached: a caller-supplied platform could be wrong for this entity.
        if (_censusContext.Platform != null)
        {
            return _censusContext.Platform;
        }

        platform = await FindPlatformAsync(censusQuery);
        if (platform != null)
        {
            await _cache.SetAsync(cacheKey, platform, _cacheExpiration);
        }

        return platform;
    }

    private async Task<Platform?> FindPlatformAsync<T>(Func<Task<T?>> censusQuery)
        where T : class
    {
        Exception? error = null;

        foreach (var platform in _options.Platforms)
        {
            try
            {
                T? result;
                using (_censusContext.Use(platform))
                {
                    result = await censusQuery();
                }

                if (result != null)
                {
                    return platform;
                }
            }
            catch (Exception ex)
            {
                error ??= ex;
            }
        }

        // Not found anywhere reachable: surface a census failure so callers can tell it from a missing entity.
        if (error != null)
        {
            ExceptionDispatchInfo.Throw(error);
        }

        return null;
    }
}

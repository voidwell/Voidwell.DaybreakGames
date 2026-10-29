using DaybreakGames.Census;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Platforms;

namespace Voidwell.DaybreakGames.Census.Collection;

public class WorldCollection : ICensusStaticCollection<CensusWorldModel>
{
    private readonly ICensusClient _client;
    private readonly ICensusContext _censusContext;
    private readonly CensusPlatformOptions _platformOptions;

    public string CollectionName => "world";

    public WorldCollection(ICensusClient censusClient, ICensusContext censusContext, IOptions<CensusPlatformOptions> platformOptions)
    {
        _client = censusClient;
        _censusContext = censusContext;
        _platformOptions = platformOptions.Value;
    }

    /// <summary>
    /// Worlds are the one collection that differs between platforms, so this returns the worlds of every enabled platform.
    /// </summary>
    public async Task<IEnumerable<CensusWorldModel>> GetCollectionAsync()
    {
        var platformWorlds = await Task.WhenAll(_platformOptions.Platforms.Select(async platform =>
        {
            IEnumerable<CensusWorldModel> worlds;
            using (_censusContext.Use(platform))
            {
                worlds = await _client.CreateQuery(CollectionName)
                    .SetLanguage("en")
                    .GetBatchAsync<CensusWorldModel>();
            }

            return worlds?.Select(world =>
            {
                world.Platform = platform;
                return world;
            }) ?? [];
        }));

        return [.. platformWorlds.SelectMany(a => a)];
    }
}

using DaybreakGames.Census;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MapCollection : ICensusCollection<CensusMapModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "map";

        public MapCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<CensusMapModel> GetMapOwnershipAsync(int worldId, int zoneId)
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .Where("world_id", a => a.Equals(worldId))
                .Where("zone_ids", a => a.Equals(zoneId))
                .GetAsync<CensusMapModel>();
        }
    }
}

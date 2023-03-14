using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class WorldEventCollection : ICensusCollection
    {
        private readonly ICensusClient _client;

        public string CollectionName => "world_event";

        public WorldEventCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusMetagameWorldEventModel>> GetMetagameWorldEventsAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .Where("type", a => a.Equals("METAGAME"))
                .GetListAsync<CensusMetagameWorldEventModel>();
        }

        public async Task<IEnumerable<CensusFacilityWorldEventModel>> GetFacilityWorldEventsByWorldIdAsync(int worldId, int? beforeTimestamp = null)
        {
            var query = _client.CreateQuery(CollectionName)
                .SetLimit(500)
                .Where("type", a => a.Equals("FACILITY"))
                .Where("world_id", a => a.Equals(worldId));

            if (beforeTimestamp.HasValue)
            {
                query.Where("before").Equals(beforeTimestamp.Value);
            }

            return await query.GetListAsync<CensusFacilityWorldEventModel>();
        }
    }
}

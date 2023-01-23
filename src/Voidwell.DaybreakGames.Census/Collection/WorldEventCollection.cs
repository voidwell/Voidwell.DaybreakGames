using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class WorldEventCollection : CensusCollection
    {
        public override string CollectionName => "world_event";

        public WorldEventCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusMetagameWorldEventModel>> GetMetagameWorldEventsAsync()
        {
            return await QueryAsync(query =>
            {
                query.Where("type").Equals("METAGAME");

                return query.GetListAsync<CensusMetagameWorldEventModel>();
            });
        }

        public async Task<IEnumerable<CensusFacilityWorldEventModel>> GetFacilityWorldEventsByWorldIdAsync(int worldId, int? beforeTimestamp = null)
        {
            return await QueryAsync(query =>
            {
                query.Where("type").Equals("FACILITY");
                query.Where("world_id").Equals(worldId);

                if (beforeTimestamp.HasValue)
                {
                    query.Where("before").Equals(beforeTimestamp.Value);
                }

                query.SetLimit(500);

                return query.GetListAsync<CensusFacilityWorldEventModel>();
            });
        }
    }
}

using DaybreakGames.Census;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MapCollection : CensusCollection
    {
        public override string CollectionName => "map";

        public MapCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<CensusMapModel> GetMapOwnershipAsync(int worldId, int zoneId)
        {
            try
            {
                return await QueryAsync(query =>
                {
                    query.SetLanguage("en");

                    query.Where("world_id").Equals(worldId);
                    query.Where("zone_ids").Equals(zoneId);

                    return query.GetAsync<CensusMapModel>();
                });
            }
            catch (Exception)
            {
                return null;
            }
            
        }
    }
}

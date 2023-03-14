using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class VehicleFactionCollection : ICensusStaticCollection<CensusVehicleFactionModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "vehicle_faction";

        public VehicleFactionCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusVehicleFactionModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("vehicle_id", "faction_id")
                .GetBatchAsync<CensusVehicleFactionModel>();
        }
    }
}

using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class VehicleCollection : ICensusStaticCollection<CensusVehicleModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "vehicle";

        public VehicleCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusVehicleModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("vehicle_id", "name", "description", "cost", "cost_resource_id", "image_id")
                .GetBatchAsync<CensusVehicleModel>();
        }
    }
}

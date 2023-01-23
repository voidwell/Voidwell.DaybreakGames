using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class VehicleCollection : CensusCollection, ICensusStaticCollection<CensusVehicleModel>
    {
        public override string CollectionName => "vehicle";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public VehicleCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusVehicleModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("vehicle_id", "name", "description", "cost", "cost_resource_id", "image_id");

                return query.GetBatchAsync<CensusVehicleModel>();
            });
        }
    }
}

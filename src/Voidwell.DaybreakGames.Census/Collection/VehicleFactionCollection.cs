using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class VehicleFactionCollection : CensusCollection, ICensusStaticCollection<CensusVehicleFactionModel>
    {
        public override string CollectionName => "vehicle_faction";

        public VehicleFactionCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusVehicleFactionModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("vehicle_id", "faction_id");

                return query.GetBatchAsync<CensusVehicleFactionModel>();
            });
        }
    }
}

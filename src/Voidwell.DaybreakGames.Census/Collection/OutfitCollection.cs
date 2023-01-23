using DaybreakGames.Census;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class OutfitCollection : CensusCollection
    {
        public override string CollectionName => "outfit";

        public OutfitCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<CensusOutfitModel> GetOutfitAsync(string outfitId)
        {
            return await QueryAsync(query =>
            {
                query.ShowFields("outfit_id", "name", "alias", "time_created", "leader_character_id", "member_count");

                query.Where("outfit_id").Equals(outfitId);

                return query.GetAsync<CensusOutfitModel>();
            });
        }
    }
}

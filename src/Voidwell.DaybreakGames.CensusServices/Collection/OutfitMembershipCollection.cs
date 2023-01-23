using DaybreakGames.Census;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class OutfitMembershipCollection : CensusCollection
    {
        public override string CollectionName => "outfit_membership";

        public OutfitMembershipCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<CensusOutfitMemberModel> GetCharacterOutfitMembershipAsync(string characterId)
        {
            return await QueryAsync(query =>
            {
                query.ShowFields("character_id", "outfit_id", "member_since_date", "rank", "rank_ordinal");
                query.Where("character_id").Equals(characterId);

                return query.GetAsync<CensusOutfitMemberModel>();
            });
        }
    }
}

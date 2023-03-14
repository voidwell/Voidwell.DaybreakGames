using DaybreakGames.Census;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class OutfitMembershipCollection : ICensusCollection<CensusOutfitMemberModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "outfit_member";

        public OutfitMembershipCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<CensusOutfitMemberModel> GetCharacterOutfitMembershipAsync(string characterId)
        {
            return await _client.CreateQuery(CollectionName)
                .ShowFields("character_id", "outfit_id", "member_since_date", "rank", "rank_ordinal")
                .Where("character_id", a => a.Equals(characterId))
                .GetAsync<CensusOutfitMemberModel>();
        }
    }
}

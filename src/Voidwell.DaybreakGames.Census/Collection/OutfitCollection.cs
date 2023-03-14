using DaybreakGames.Census;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class OutfitCollection : ICensusCollection<CensusOutfitModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "outfit";

        public OutfitCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<CensusOutfitModel> GetOutfitAsync(string outfitId)
        {
            return await _client.CreateQuery(CollectionName)
                .ShowFields("outfit_id", "name", "alias", "time_created", "leader_character_id", "member_count")
                .Where("outfit_id", a => a.Equals(outfitId))
                .GetAsync<CensusOutfitModel>();
        }
    }
}

using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusOutfit
    {
        private readonly ICensusClient _censusClient;

        public CensusOutfit(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<CensusOutfitModel> GetOutfit(string outfitId)
        {
            var query = _censusClient.CreateQuery("outfit");

            query.ShowFields(new[]
            {
                "outfit_id",
                "name",
                "alias",
                "time_created",
                "leader_character_id",
                "member_count"
            });

            query.Where("outfit_id").Equals(outfitId);

            return await query.Get<CensusOutfitModel>();
        }
    }
}

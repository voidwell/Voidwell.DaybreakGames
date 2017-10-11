using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusOutfit
    {
        public static async Task<CensusOutfitModel> GetOutfit(string outfitId)
        {
            var query = new CensusQuery.Query("outfit");

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

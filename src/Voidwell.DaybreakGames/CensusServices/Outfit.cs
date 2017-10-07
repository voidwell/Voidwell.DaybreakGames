using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class Outfit
    {
        public static async Task<JToken> GetOutfit(string outfitId)
        {
            var query = new CensusQuery.Query("outfit");

            query.ShowFields(new[]
            {
                "outfit_id",
                "name",
                "alias",
                "time_created_date",
                "leader_character_id",
                "member_count"
            });

            query.Where("outfit_id").Equals(outfitId);

            return (await query.Get()).First;
        }
    }
}

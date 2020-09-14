using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusOutfit
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusOutfit(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<CensusOutfitModel> GetOutfit(string outfitId)
        {
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "time_created", "leader_character_id", "member_count");

            query.Where("outfit_id").Equals(outfitId);

            return await query.GetAsync<CensusOutfitModel>();
        }

        public async Task<CensusOutfitMemberModel> GetCharacterOutfitMembership(string characterId)
        {
            var query = _queryFactory.Create("outfit_member");

            query.ShowFields("character_id", "outfit_id", "member_since_date", "rank", "rank_ordinal");
            query.Where("character_id").Equals(characterId);

            return await query.GetAsync<CensusOutfitMemberModel>();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusFaction
    {
        private readonly ICensusClient _censusClient;

        public CensusFaction(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusFactionModel>> GetAllFactions()
        {
            var query = _censusClient.CreateQuery("faction");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "factiion_id",
                "name",
                "image_id",
                "code_tag",
                "user_selectable"
            });

            return await query.GetBatch<CensusFactionModel>();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusTitle
    {
        private readonly ICensusClient _censusClient;

        public CensusTitle(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusTitleModel>> GetAllTitles()
        {
            var query = _censusClient.CreateQuery("title");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "title_id",
                "name"
            });

            return await query.GetBatch<CensusTitleModel>();
        }
    }
}

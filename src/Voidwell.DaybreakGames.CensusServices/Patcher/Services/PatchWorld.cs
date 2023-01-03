using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices.Patcher.Services
{
    public class PatchWorld : ICensusWorld
    {
        private readonly IPatchClient _patchClient;
        private readonly CensusWorld _censusClient;

        public PatchWorld(IPatchClient client, CensusWorld censusWorld)
        {
            _patchClient = client;
            _censusClient = censusWorld;
        }

        public async Task<IEnumerable<CensusWorldModel>> GetAllWorlds()
        {
            var censusResults = await _censusClient.GetAllWorlds();

            var query = _patchClient.CreateQuery("world");
            query.SetLanguage("en");

            var patchResults = await query.GetBatchAsync<CensusWorldModel>();

            return PatchUtil.PatchData<CensusWorldModel>(x => x.WorldId, censusResults, patchResults);
        }
    }
}

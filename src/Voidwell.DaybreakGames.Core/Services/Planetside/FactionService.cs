using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Services;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public class FactionService : IFactionService
    {
        public readonly IFactionRepository _factionRepository;
        private readonly CensusFaction _censusFaction;

        public FactionService(IFactionRepository factionRepository, CensusFaction censusFaction)
        {
            _factionRepository = factionRepository;
            _censusFaction = censusFaction;
        }

        public async Task RefreshStore()
        {
            var factions = await _censusFaction.GetAllFactions();

            if (factions != null)
            {
                await _factionRepository.UpsertRangeAsync(factions.Select(ConvertToDbModel));
            }
        }        

        private static Faction ConvertToDbModel(CensusFactionModel censusModel)
        {
            return new Faction
            {
                Id = censusModel.FactionId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId,
                CodeTag = censusModel.CodeTag,
                UserSelectable = censusModel.UserSelectable
            };
        }
    }
}

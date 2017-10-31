using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class FactionService : IFactionService
    {
        public readonly Func<PS2DbContext> _dbContextFactory;
        private readonly CensusFaction _censusFaction;

        public string ServiceName => "FactionService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public FactionService(Func<PS2DbContext> dbContextFactory, CensusFaction censusFaction)
        {
            _dbContextFactory = dbContextFactory;
            _censusFaction = censusFaction;
        }

        public async Task RefreshStore()
        {
            var factions = await _censusFaction.GetAllFactions();

            if (factions != null)
            {
                var dbContext = _dbContextFactory();
                await dbContext.Factions.UpsertRangeAsync(factions.Select(i => ConvertToDbModel(i)));
                await dbContext.SaveChangesAsync();
            }
        }

        private DbFaction ConvertToDbModel(CensusFactionModel censusModel)
        {
            return new DbFaction
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

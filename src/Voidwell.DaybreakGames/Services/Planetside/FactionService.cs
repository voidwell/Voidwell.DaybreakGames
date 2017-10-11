using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class FactionService : IFactionService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;

        public FactionService(PS2DbContext ps2DbContext)
        {
            _ps2DbContext = ps2DbContext;
        }

        public async Task RefreshStore()
        {
            var factions = await CensusFaction.GetAllFactions();

            if (factions != null)
            {
                _ps2DbContext.UpdateRange(factions.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
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

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}

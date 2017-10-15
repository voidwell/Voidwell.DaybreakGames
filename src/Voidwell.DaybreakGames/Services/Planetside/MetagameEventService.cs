using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MetagameEventService : IMetagameEventService , IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusMetagameEvent _censusMetagameEvent;

        public MetagameEventService(PS2DbContext ps2DbContext, CensusMetagameEvent censusMetagameEvent)
        {
            _ps2DbContext = ps2DbContext;
            _censusMetagameEvent = censusMetagameEvent;
        }

        public async Task RefreshStore()
        {
            var categories = await _censusMetagameEvent.GetAllCategories();
            var states = await _censusMetagameEvent.GetAllStates();

            if (categories != null)
            {
                _ps2DbContext.MetagameEventCategories.UpdateRange(categories.Select(i => ConvertToDbModel(i)));
            }

            if (states != null)
            {
                _ps2DbContext.MetagameEventStates.UpdateRange(states.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbMetagameEventCategory ConvertToDbModel(CensusMetagameEventCategoryModel model)
        {
            return new DbMetagameEventCategory
            {
                Id = model.MetagameEventId,
                Name = model.Name?.English,
                Description = model.Description?.English,
                Type = model.Type,
                ExperienceBonus = model.ExperienceBonus
            };
        }

        private DbMetagameEventState ConvertToDbModel(CensusMetagameEventStateModel model)
        {
            return new DbMetagameEventState
            {
                Id = model.MetagameEventStateId,
                Name = model.Name
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MetagameEventService : IMetagameEventService
    {
        private readonly IMetagameEventRepository _metagameEventRepository;
        private readonly CensusMetagameEvent _censusMetagameEvent;

        public string ServiceName => "MetagameEventService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public MetagameEventService(IMetagameEventRepository metagameEventRepository, CensusMetagameEvent censusMetagameEvent)
        {
            _metagameEventRepository = metagameEventRepository;
            _censusMetagameEvent = censusMetagameEvent;
        }

        public async Task RefreshStore()
        {
            var categories = await _censusMetagameEvent.GetAllCategories();
            var states = await _censusMetagameEvent.GetAllStates();

            if (categories != null)
            {
                await _metagameEventRepository.UpsertRangeAsync(categories.Select(ConvertToDbModel));
            }

            if (states != null)
            {
                await _metagameEventRepository.UpsertRangeAsync(states.Select(ConvertToDbModel));
            }
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
    }
}

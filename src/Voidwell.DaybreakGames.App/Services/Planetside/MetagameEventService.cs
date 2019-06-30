using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MetagameEventService : IMetagameEventService
    {
        private readonly IMetagameEventRepository _metagameEventRepository;
        private readonly CensusMetagameEvent _censusMetagameEvent;

        public string ServiceName => "MetagameEventService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private static readonly Dictionary<int, TimeSpan> _metagameTypeDurations = new Dictionary<int, TimeSpan>
        {
            {1, TimeSpan.FromMinutes(80) },
            {2, TimeSpan.FromMinutes(45) },
            {5, TimeSpan.FromMinutes(5) },
            {8, TimeSpan.FromMinutes(45) },
            {9, TimeSpan.FromMinutes(90) },
            {10, TimeSpan.FromMinutes(25) }
        };

        public MetagameEventService(IMetagameEventRepository metagameEventRepository, CensusMetagameEvent censusMetagameEvent)
        {
            _metagameEventRepository = metagameEventRepository;
            _censusMetagameEvent = censusMetagameEvent;
        }

        public async Task<ZoneMetagameEvent> GetMetagameEvent(int metagameEventId)
        {
            var categoryTask = _metagameEventRepository.GetMetagameEventCategory(metagameEventId);
            var zoneTask = _metagameEventRepository.GetMetagameCategoryZoneId(metagameEventId);

            await Task.WhenAll(categoryTask, zoneTask);

            if (categoryTask.Result == null)
            {
                return null;
            }

            var category = categoryTask.Result;

            return new ZoneMetagameEvent
            {
                Id = category.Id,
                TypeId = category.Type.GetValueOrDefault(),
                Name = category.Name,
                Description = category.Description,
                ZoneId = zoneTask.Result,
                Duration = _metagameTypeDurations.GetValueOrDefault(category.Type.GetValueOrDefault(), TimeSpan.FromMinutes(45))
            };
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

        private static MetagameEventCategory ConvertToDbModel(CensusMetagameEventCategoryModel model)
        {
            return new MetagameEventCategory
            {
                Id = model.MetagameEventId,
                Name = model.Name?.English,
                Description = model.Description?.English,
                Type = model.Type,
                ExperienceBonus = model.ExperienceBonus
            };
        }

        private static MetagameEventState ConvertToDbModel(CensusMetagameEventStateModel model)
        {
            return new MetagameEventState
            {
                Id = model.MetagameEventStateId,
                Name = model.Name
            };
        }
    }
}

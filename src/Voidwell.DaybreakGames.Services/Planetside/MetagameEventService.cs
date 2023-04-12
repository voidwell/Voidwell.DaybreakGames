using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MetagameEventService : IMetagameEventService
    {
        private readonly IMetagameEventStore _metagameEventStore;

        private static readonly Dictionary<int, TimeSpan> _metagameTypeDurations = new Dictionary<int, TimeSpan>
        {
            {1, TimeSpan.FromMinutes(80) },
            {2, TimeSpan.FromMinutes(45) },
            {5, TimeSpan.FromMinutes(5) },
            {8, TimeSpan.FromMinutes(45) },
            {9, TimeSpan.FromMinutes(90) },
            {10, TimeSpan.FromMinutes(25) }
        };

        public MetagameEventService(IMetagameEventStore metagameEventStore)
        {
            _metagameEventStore = metagameEventStore;
        }

        public async Task<ZoneMetagameEvent> GetMetagameEvent(int metagameEventId)
        {
            var categoryTask = _metagameEventStore.GetMetagameEventCategoryAsync(metagameEventId);
            var zoneTask = _metagameEventStore.GetMetagameCategoryZoneIdAsync(metagameEventId);

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
    }
}

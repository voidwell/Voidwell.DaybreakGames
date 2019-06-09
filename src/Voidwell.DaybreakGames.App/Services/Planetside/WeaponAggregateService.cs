using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponAggregateService : IWeaponAggregateService
    {
        private readonly IWeaponAggregateRepository _weaponAggregateRepository;
        private readonly ICache _cache;
        private readonly ILogger<WeaponAggregateService> _logger;

        private const string _cacheKey = "ps2.weaponAggregates";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(8);

        public WeaponAggregateService(IWeaponAggregateRepository weaponAggregateRepository, ICache cache, ILogger<WeaponAggregateService> logger)
        {
            _weaponAggregateRepository = weaponAggregateRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeaponAggregate> GetAggregateForItem(int itemId)
        {
            var cacheKey = $"{_cacheKey}_{itemId}";

            var aggregate = await _cache.GetAsync<WeaponAggregate>(cacheKey);
            if (aggregate != null)
            {
                return aggregate;
            }

            aggregate = await _weaponAggregateRepository.GetWeaponAggregateByItemId(itemId);
            if (aggregate != null)
            {
                await _cache.SetAsync(cacheKey, aggregate, _cacheExpiration);
            }

            return aggregate;
        }

        public async Task<Dictionary<string, WeaponAggregate>> GetAggregates(IEnumerable<int> itemIds)
        {
            var aggregates = await Task.WhenAll(itemIds.Distinct().Select(GetAggregateForItem));

            return aggregates.Where(a => a != null).ToDictionary(a => $"{a.ItemId}-{a.VehicleId}", a => a);
        }
    }
}

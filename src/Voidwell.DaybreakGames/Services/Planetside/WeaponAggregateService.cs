using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using static Voidwell.DaybreakGames.Data.Repositories.CharacterRepository;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponAggregateService : IWeaponAggregateService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.weaponAggregate";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

        private SemaphoreSlim _aggregateLock = new SemaphoreSlim(1);

        public WeaponAggregateService(ICharacterRepository characterRepository, ICache cache)
        {
            _characterRepository = characterRepository;
            _cache = cache;
        }

        public async Task<WeaponAggregate> GetAggregateForItem(int itemId)
        {
            var aggregates = await GetAggregates();

            if (aggregates != null && aggregates.TryGetValue($"{itemId}-0", out var aggregate))
            {
                return aggregate;
            }

            return null;
        }

        public async Task<Dictionary<string, WeaponAggregate>> GetAggregates()
        {
            await _aggregateLock.WaitAsync();

            try
            {
                var aggregates = await _cache.GetAsync<Dictionary<string, WeaponAggregate>>(_cacheKey);
                if (aggregates != null)
                {
                    return aggregates;
                }

                var calculatedAggregates = await _characterRepository.GetWeaponAggregates();
                if (calculatedAggregates == null)
                {
                    return null;
                }

                aggregates = calculatedAggregates.ToDictionary(a => $"{a.ItemId}-{a.VehicleId}", a => a);

                await _cache.SetAsync(_cacheKey, aggregates, _cacheExpiration);
                return aggregates;
            }
            finally
            {
                _aggregateLock.Release();
            }
        }
    }
}

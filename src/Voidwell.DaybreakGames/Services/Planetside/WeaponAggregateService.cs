using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Data.Repositories.Models;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponAggregateService : IWeaponAggregateService
    {
        private readonly IFunctionalRepository _functionalRepository;
        private readonly ICache _cache;
        private readonly ILogger<WeaponAggregateService> _logger;

        private const string _cacheKey = "ps2.weaponAggregates";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(7);

        private SemaphoreSlim _aggregateLock = new SemaphoreSlim(1);
        private bool fetchingAggregates = false;

        public WeaponAggregateService(IFunctionalRepository functionalRepository, ICache cache, ILogger<WeaponAggregateService> logger)
        {
            _functionalRepository = functionalRepository;
            _cache = cache;
            _logger = logger;
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
                var aggregates = await _cache.GetAsync<WeaponAggregates>(_cacheKey);
                if (aggregates == null)
                {
                    aggregates = await LoadWeaponAggregatesAsync();
                }
                else if (DateTime.UtcNow - aggregates.CalculatedDate >= TimeSpan.FromDays(1))
                {
                    LoadWeaponAggregatesParallel();
                }

                return aggregates?.Aggregates;
            }
            finally
            {
                _aggregateLock.Release();
            }
        }

        private void LoadWeaponAggregatesParallel()
        {
            if (!fetchingAggregates)
                Task.Run(() => LoadWeaponAggregatesAsync());
        }

        private async Task<WeaponAggregates> LoadWeaponAggregatesAsync()
        {
            fetchingAggregates = true;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var calculatedAggregates = await _functionalRepository.GetWeaponAggregates();
                if (calculatedAggregates == null)
                {
                    return null;
                }

                var aggregates = new WeaponAggregates
                {
                    Aggregates = calculatedAggregates.ToDictionary(a => $"{a.ItemId}-{a.VehicleId}", a => a),
                    CalculatedDate = DateTime.UtcNow
                };

                await _cache.SetAsync(_cacheKey, aggregates, _cacheExpiration);

                return aggregates;
            }
            finally
            {
                fetchingAggregates = false;
                stopwatch.Stop();
                _logger.LogInformation($"Loaded weapon aggregates in {stopwatch.ElapsedMilliseconds} ms");
            }
        }

    }
}

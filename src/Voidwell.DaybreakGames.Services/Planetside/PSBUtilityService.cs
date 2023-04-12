using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Data.Repositories.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class PSBUtilityService : IPSBUtilityService
    {
        private readonly IFunctionalRepository _functionalRepository;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.psb-online-accounts";
        private readonly TimeSpan _lastOnlineAccountsExpiration = TimeSpan.FromMinutes(5);

        private readonly SemaphoreSlim _lastOnlineAccountsLock = new SemaphoreSlim(1);

        public PSBUtilityService(IFunctionalRepository functionalRepository, ICache cache)
        {
            _functionalRepository = functionalRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<CharacterLastSession>> GetLastOnlinePSBAccounts()
        {
            await _lastOnlineAccountsLock.WaitAsync();

            try
            {
                var lastOnlineResults = await _cache.GetAsync<IEnumerable<CharacterLastSession>>(_cacheKey);
                if (lastOnlineResults != null)
                {
                    return lastOnlineResults;
                }

                lastOnlineResults = await _functionalRepository.GetPSBLastOnline();

                if (lastOnlineResults != null)
                {
                    await _cache.SetAsync(_cacheKey, lastOnlineResults, _lastOnlineAccountsExpiration);
                }

                return lastOnlineResults;
            }
            finally
            {
                _lastOnlineAccountsLock.Release();
            }
        }
    }
}

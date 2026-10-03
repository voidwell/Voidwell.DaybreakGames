using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Data.Repositories.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

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

    public async Task<IEnumerable<CharacterLastSession>?> GetLastOnlinePSBAccounts()
    {
        await _lastOnlineAccountsLock.WaitAsync();

        try
        {
            return await _cache.GetOrSetAsync<IEnumerable<CharacterLastSession>>(_cacheKey, ct => _functionalRepository.GetPSBLastOnline(), _lastOnlineAccountsExpiration);
        }
        finally
        {
            _lastOnlineAccountsLock.Release();
        }
    }
}

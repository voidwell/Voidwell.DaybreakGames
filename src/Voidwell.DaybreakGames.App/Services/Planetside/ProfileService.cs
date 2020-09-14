using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ProfileService : IProfileService, IDisposable
    {
        private readonly IProfileStore _profileStore;

        private Dictionary<int, Profile> _loadoutMapping = new Dictionary<int, Profile>();

        private readonly SemaphoreSlim _loadoutSemaphore = new SemaphoreSlim(1);

        public ProfileService(IProfileStore profileStore)
        {
            _profileStore = profileStore;
        }

        public Task<IEnumerable<Profile>> GetAllProfiles()
        {
            return _profileStore.GetAllProfilesAsync();
        }

        public async Task<Profile> GetProfileFromLoadoutIdAsync(int loadoutId)
        {
            if (_loadoutMapping == null || _loadoutMapping.Count == 0)
            {
                await SetupLoadoutMap();
            }

            return _loadoutMapping.GetValueOrDefault(loadoutId, null);
        }

        private async Task SetupLoadoutMap()
        {
            await _loadoutSemaphore.WaitAsync();

            try
            {
                if (_loadoutMapping != null && _loadoutMapping.Count > 0)
                {
                    return;
                }

                var loadoutsTask = _profileStore.GetAllLoadoutsAsync();
                var profilesTask = _profileStore.GetAllProfilesAsync();

                await Task.WhenAll(loadoutsTask, profilesTask);

                var loadouts = loadoutsTask.Result;
                var profiles = profilesTask.Result;

                _loadoutMapping = loadouts.ToDictionary(a => a.Id, a => profiles.FirstOrDefault(b => a.ProfileId == b.Id));
            }
            finally
            {
                _loadoutSemaphore.Release();
            }
        }

        public void Dispose()
        {
            _loadoutSemaphore.Dispose();
        }
    }
}

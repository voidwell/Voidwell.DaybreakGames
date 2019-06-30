using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ProfileService : IProfileService, IDisposable
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILoadoutRepository _loadoutRepository;
        private readonly CensusProfile _censusProfile;
        private readonly CensusLoadout _censusLoadout;

        public string ServiceName => "ProfileService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private Dictionary<int, Profile> _loadoutMapping = new Dictionary<int, Profile>();

        private readonly SemaphoreSlim _loadoutSemaphore = new SemaphoreSlim(1);

        public ProfileService(IProfileRepository profileRepository, ILoadoutRepository loadoutRepository, CensusProfile censusProfile, CensusLoadout censusLoadout)
        {
            _profileRepository = profileRepository;
            _loadoutRepository = loadoutRepository;
            _censusProfile = censusProfile;
            _censusLoadout = censusLoadout;
        }

        public Task<IEnumerable<Profile>> GetAllProfiles()
        {
            return _profileRepository.GetAllProfilesAsync();
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

                var loadoutsTask = _loadoutRepository.GetAllLoadoutsAsync();
                var profilesTask = _profileRepository.GetAllProfilesAsync();

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

        public async Task RefreshStore()
        {
            var profiles = await _censusProfile.GetAllProfiles();

            if (profiles != null)
            {
                await _profileRepository.UpsertRangeAsync(profiles.Select(ConvertToDbModel));
            }

            var loadouts = await _censusLoadout.GetAllLoadouts();

            if (loadouts != null)
            {
                await _loadoutRepository.UpsertRangeAsync(loadouts.Select(ConvertToDbModel));
            }
        }

        private Profile ConvertToDbModel(CensusProfileModel censusModel)
        {
            return new Profile
            {
                Id = censusModel.ProfileId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId,
                ProfileTypeId = censusModel.ProfileTypeId,
                FactionId = censusModel.FactionId
            };
        }

        private Loadout ConvertToDbModel(CensusLoadoutModel censusModel)
        {
            return new Loadout
            {
                Id = censusModel.LoadoutId,
                ProfileId = censusModel.ProfileId,
                FactionId = censusModel.FactionId,
                CodeName = censusModel.CodeName
            };
        }

        public void Dispose()
        {
            _loadoutSemaphore.Dispose();
        }
    }
}

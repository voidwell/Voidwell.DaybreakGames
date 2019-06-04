using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILoadoutRepository _loadoutRepository;
        private readonly CensusProfile _censusProfile;
        private readonly CensusLoadout _censusLoadout;
        private readonly ICache _cache;

        public string ServiceName => "ProfileService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private const string _cacheKeyPrefix = "ps2.profileService";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private Dictionary<int, int> _loadoutMapping = new Dictionary<int, int>();

        public ProfileService(IProfileRepository profileRepository, ILoadoutRepository loadoutRepository, CensusProfile censusProfile, CensusLoadout censusLoadout, ICache cache)
        {
            _profileRepository = profileRepository;
            _loadoutRepository = loadoutRepository;
            _censusProfile = censusProfile;
            _censusLoadout = censusLoadout;
            _cache = cache;
        }

        public Task<IEnumerable<Profile>> GetAllProfiles()
        {
            return _profileRepository.GetAllProfilesAsync();
        }

        public async Task<int> GetProfileIdFromLoadoutAsync(int loadoutId)
        {
            if (_loadoutMapping == null || _loadoutMapping.Count == 0)
            {
                var loadouts = await GetAllLoadouts();
                _loadoutMapping = loadouts.ToDictionary(a => a.Id, a => a.ProfileId);
            }

            return _loadoutMapping.GetValueOrDefault(loadoutId, loadoutId);
        }

        private async Task<IEnumerable<Loadout>> GetAllLoadouts()
        {
            var cacheKey = $"{_cacheKeyPrefix}_loadouts";

            var loadouts = await _cache.GetAsync<IEnumerable<Loadout>>(cacheKey);
            if (loadouts != null)
            {
                return loadouts;
            }

            loadouts = await _loadoutRepository.GetAllLoadoutsAsync();

            if (loadouts != null)
            {
                await _cache.SetAsync(cacheKey, loadouts, _cacheExpiration);
            }

            return loadouts;
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
    }
}

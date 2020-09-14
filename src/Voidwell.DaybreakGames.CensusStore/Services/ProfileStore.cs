using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class ProfileStore : IProfileStore
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILoadoutRepository _loadoutRepository;
        private readonly CensusProfile _censusProfile;
        private readonly CensusLoadout _censusLoadout;

        public ProfileStore(IProfileRepository profileRepository, ILoadoutRepository loadoutRepository, CensusProfile censusProfile, CensusLoadout censusLoadout)
        {
            _profileRepository = profileRepository;
            _loadoutRepository = loadoutRepository;
            _censusProfile = censusProfile;
            _censusLoadout = censusLoadout;
        }

        public string StoreName => "ProfileStore";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            return _profileRepository.GetAllProfilesAsync();
        }

        public Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            return _loadoutRepository.GetAllLoadoutsAsync();
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

        private static Profile ConvertToDbModel(CensusProfileModel censusModel)
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

        private static Loadout ConvertToDbModel(CensusLoadoutModel censusModel)
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

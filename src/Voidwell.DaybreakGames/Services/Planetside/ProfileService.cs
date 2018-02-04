using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly CensusProfile _censusProfile;

        public string ServiceName => "ProfileService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ProfileService(IProfileRepository profileRepository, CensusProfile censusProfile)
        {
            _profileRepository = profileRepository;
            _censusProfile = censusProfile;
        }

        public Task<IEnumerable<Profile>> GetAllProfiles()
        {
            return _profileRepository.GetAllProfilesAsync();
        }

        public async Task RefreshStore()
        {
            var profiles = await _censusProfile.GetAllProfiles();

            if (profiles != null)
            {
                await _profileRepository.UpsertRangeAsync(profiles.Select(ConvertToDbModel));
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
    }
}

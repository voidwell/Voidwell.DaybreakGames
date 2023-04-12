using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class ProfileStore : IProfileStore
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileStore(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            return _profileRepository.GetAllProfilesAsync();
        }
    }
}

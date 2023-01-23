using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class ProfileStore : IProfileStore
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ProfileCollection _profileCollection;
        private readonly AutoMapper.IMapper _mapper; 

        public ProfileStore(IProfileRepository profileRepository, ProfileCollection profileCollection, AutoMapper.IMapper mapper)
        {
            _profileRepository = profileRepository;
            _profileCollection = profileCollection;
            _mapper = mapper;
        }

        public string StoreName => "ProfileStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            return _profileRepository.GetAllProfilesAsync();
        }

        public async Task RefreshStore()
        {
            var profiles = await _profileCollection.GetCollectionAsync();

            if (profiles != null)
            {
                await _profileRepository.UpsertRangeAsync(profiles.Select(_mapper.Map<Profile>));
            }
        }
    }
}

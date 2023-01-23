using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class ExperienceStore : IExperienceStore
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly ExperienceCollection _experienceCollection;
        private readonly IMapper _mapper;

        public string StoreName => "ExperienceStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public ExperienceStore(IExperienceRepository experienceRepository, ExperienceCollection experienceCollection, IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _experienceCollection = experienceCollection;
            _mapper = mapper;
        }

        public async Task RefreshStore()
        {
            var allExperience = await _experienceCollection.GetCollectionAsync();
            if (allExperience != null)
            {
                await _experienceRepository.UpsertRangeAsync(allExperience.Select(_mapper.Map<Experience>));
            }
        }
    }
}

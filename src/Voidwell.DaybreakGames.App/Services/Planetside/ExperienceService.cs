using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ExperienceService : IExperienceService
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly CensusExperience _censusExperience;

        public string ServiceName => "ExperienceService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(45);

        public ExperienceService(IExperienceRepository experienceRepository, CensusExperience censusExperience)
        {
            _experienceRepository = experienceRepository;
            _censusExperience = censusExperience;
        }

        public async Task RefreshStore()
        {
            var allExperience = await _censusExperience.GetAllExperience();
            if (allExperience != null)
            {
                await _experienceRepository.UpsertRangeAsync(allExperience.Select(ConvertToDbModel));
            }
        }

        private static Experience ConvertToDbModel(CensusExperienceModel experience)
        {
            return new Experience
            {
                Id = experience.ExperienceId,
                Description = experience.Description,
                Xp = experience.Xp
            };
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class TitleStore : ITitleStore
    {
        private readonly ITitleRepository _titleRepository;
        private readonly CensusTitle _censusTitle;

        public string StoreName => "TitleStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public TitleStore(ITitleRepository titleRepository, CensusTitle censusTitle)
        {
            _titleRepository = titleRepository;
            _censusTitle = censusTitle;
        }

        public async Task RefreshStore()
        {
            var profiles = await _censusTitle.GetAllTitles();

            if (profiles != null)
            {
                await _titleRepository.UpdateRangeAsync(profiles.Select(ConvertToDbModel));
            }
        }

        private static Title ConvertToDbModel(CensusTitleModel censusModel)
        {
            return new Title
            {
                Id = censusModel.TitleId,
                Name = censusModel.Name.English
            };
        }
    }
}

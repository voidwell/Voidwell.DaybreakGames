﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class TitleService : ITitleService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;

        public TitleService(PS2DbContext ps2DbContext)
        {
            _ps2DbContext = ps2DbContext;
        }

        public async Task RefreshStore()
        {
            var profiles = await CensusTitle.GetAllTitles();

            if (profiles != null)
            {
                _ps2DbContext.Titles.UpdateRange(profiles.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbTitle ConvertToDbModel(CensusTitleModel censusModel)
        {
            return new DbTitle
            {
                Id = censusModel.TitleId,
                Name = censusModel.Name.English,
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
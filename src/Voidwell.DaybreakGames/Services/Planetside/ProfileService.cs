using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ProfileService : IProfileService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusProfile _censusProfile;

        public string ServiceName => "ProfileService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ProfileService(PS2DbContext ps2DbContext, CensusProfile censusProfile)
        {
            _ps2DbContext = ps2DbContext;
            _censusProfile = censusProfile;
        }

        public async Task<IEnumerable<DbProfile>> GetAllProfiles()
        {
            return await _ps2DbContext.Profiles.Where(p => p != null)
                .ToListAsync();
        }

        public async Task RefreshStore()
        {
            var profiles = await _censusProfile.GetAllProfiles();

            if (profiles != null)
            {
                _ps2DbContext.UpdateRange(profiles.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbProfile ConvertToDbModel(CensusProfileModel censusModel)
        {
            return new DbProfile
            {
                Id = censusModel.FactionId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId,
                ProfileTypeId = censusModel.ProfileTypeId,
                FactionId = censusModel.FactionId
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}

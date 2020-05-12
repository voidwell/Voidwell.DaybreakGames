using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IProfileService
    {
        Task RefreshStore();
        Task<IEnumerable<Profile>> GetAllProfiles();
        Task<Profile> GetProfileFromLoadoutIdAsync(int loadoutId);
    }
}
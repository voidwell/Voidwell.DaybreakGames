using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Services
{
    public interface IPSBUtilityService
    {
        Task<IEnumerable<CharacterLastSession>> GetLastOnlinePSBAccounts();
    }
}
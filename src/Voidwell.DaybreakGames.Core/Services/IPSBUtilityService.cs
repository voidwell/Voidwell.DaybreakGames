using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Core.Services
{
    public interface IPSBUtilityService
    {
        Task<IEnumerable<CharacterLastSession>> GetLastOnlinePSBAccounts();
    }
}
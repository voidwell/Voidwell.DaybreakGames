using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Api.Authentication
{
    public interface IUserRolesClient
    {
        Task<IEnumerable<string>> GetRolesAsync(Guid userId);
    }
}

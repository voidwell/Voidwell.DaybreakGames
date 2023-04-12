using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface IDirectiveService
    {
        Task<IEnumerable<DirectiveTreeCategory>> GetDirectiveDataAsync();
    }
}
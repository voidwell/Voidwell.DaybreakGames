using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ITitleRepository
    {
        Task UpdateRangeAsync(IEnumerable<DbTitle> entities);
    }
}

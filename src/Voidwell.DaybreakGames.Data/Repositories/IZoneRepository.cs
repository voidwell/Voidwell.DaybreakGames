using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IZoneRepository
    {
        Task<IEnumerable<DbZone>> GetAllZonesAsync();
        Task UpsertRangeAsync(IEnumerable<DbZone> entities);
    }
}

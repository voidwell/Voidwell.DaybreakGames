using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IZoneRepository
    {
        Task<IEnumerable<Zone>> GetAllZonesAsync();
        Task<IEnumerable<Zone>> GetZonesByIdsAsync(params int[] zoneIds);
        Task UpsertRangeAsync(IEnumerable<Zone> entities);
    }
}

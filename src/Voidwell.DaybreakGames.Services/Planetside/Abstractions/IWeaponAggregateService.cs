using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface IWeaponAggregateService
    {
        Task<WeaponAggregate> GetAggregateForItem(int itemId);
        Task<Dictionary<string, WeaponAggregate>> GetAggregates(IEnumerable<int> itemIds);
    }
}
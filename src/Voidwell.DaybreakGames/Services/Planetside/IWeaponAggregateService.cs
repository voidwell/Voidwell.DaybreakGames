using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWeaponAggregateService
    {
        Task<WeaponAggregate> GetAggregateForItem(int itemId);
        Task<Dictionary<string, WeaponAggregate>> GetAggregates();
    }
}
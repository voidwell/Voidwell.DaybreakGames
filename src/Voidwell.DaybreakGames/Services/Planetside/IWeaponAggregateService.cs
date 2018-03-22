using System.Collections.Generic;
using System.Threading.Tasks;
using static Voidwell.DaybreakGames.Data.Repositories.CharacterRepository;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWeaponAggregateService
    {
        Task<WeaponAggregate> GetAggregateForItem(int itemId);
        Task<Dictionary<string, WeaponAggregate>> GetAggregates();
    }
}
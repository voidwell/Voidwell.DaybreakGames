using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public interface ICensusItem
    {
        Task<IEnumerable<CensusItemModel>> GetAllItems();
        Task<CensusWeaponInfoModel> GetWeaponInfo(int weaponItemId);
    }
}
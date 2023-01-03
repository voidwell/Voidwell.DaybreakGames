using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices.Patcher.Services
{
    public class PatchItem : ICensusItem
    {
        private readonly IPatchClient _patchClient;
        private readonly CensusItem _censusClient;

        public PatchItem(IPatchClient client, CensusItem censusItem)
        {
            _patchClient = client;
            _censusClient = censusItem;
        }

        public async Task<IEnumerable<CensusItemModel>> GetAllItems()
        {
            var censusResults = await _censusClient.GetAllItems();

            var query = _patchClient.CreateQuery("item");
            query.SetLanguage("en");

            query.ShowFields("item_id", "item_type_id", "item_category_id", "is_vehicle_weapon", "name", "description", "faction_id", "max_stack_size", "image_id");

            var patchResults = await query.GetBatchAsync<CensusItemModel>();

            return PatchUtil.PatchData<CensusItemModel>(x => x.ItemId, censusResults, patchResults);
        }

        public Task<CensusWeaponInfoModel> GetWeaponInfo(int weaponItemId)
        {
            return _censusClient.GetWeaponInfo(weaponItemId);
        }
    }
}

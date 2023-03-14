using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ItemCollection : CensusPatchCollection, ICensusStaticCollection<CensusItemModel>
    {
        public string CollectionName => "item";

        public ItemCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusItemModel>> GetCollectionAsync()
        {
            return await QueryAsync(CollectionName, query =>
                query.SetLanguage("en")
                    .ShowFields("item_id", "item_type_id", "item_category_id", "is_vehicle_weapon", "name", "description", "faction_id", "max_stack_size", "image_id")
                    .GetBatchAsync<CensusItemModel>());
        }

        public async Task<CensusWeaponInfoModel> GetWeaponInfoAsync(int weaponItemId)
        {
            return await _censusClient.CreateQuery(CollectionName)
                .SetLanguage("en")
                .HideFields("image_set_id", "image_path", "skill_set_id", "is_default_attachment", "passive_ability_id", "activatable_ability_id", "item_type_id")
                .Where("item_id", a => a.Equals(weaponItemId.ToString()))
                .JoinService("weapon", weapon =>
                    weapon.WithInjectAt("weapon")
                        .OnField("item_id")
                        .ToField("weapon_id")
                        .HideFields("weapon_id", "weapon_group_id"))
                .JoinService("weapon_datasheet", wepDat =>
                    wepDat.WithInjectAt("datasheet")
                        .Where("item_id", a => a.Equals(weaponItemId.ToString()))
                        .HideFields("show_clip_size", "show_fire_modes", "show_range"))
                .JoinService("item_category", itemCategory =>
                    itemCategory.WithInjectAt("category")
                        .OnField("item_category_id"))
                .JoinService("fire_mode", fireMode =>
                    fireMode.WithInjectAt("fire_mode")
                        .OnField("item_id")
                        .IsList(true)
                        .HideFields("pellets_per_shot", "pellet_spread", "max_speed", "projectile_description", "damage_type", "damage_target_type", "damage_resist_type")
                        .JoinService("player_state_group", playerState =>
                            playerState.OnField("player_state_group_id")
                                .IsList(true)
                                .WithInjectAt("states")))
                .GetAsync<CensusWeaponInfoModel>();
        }
    }
}

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
                .JoinService("weapon", a =>
                {
                    a.WithInjectAt("weapon");
                    a.OnField("item_id");
                    a.ToField("weapon_id");
                    a.HideFields("weapon_id", "weapon_group_id");
                })
                .JoinService("weapon_datasheet", a =>
                {
                    a.WithInjectAt("datasheet");
                    a.Where("item_id").Equals(weaponItemId.ToString());
                    a.HideFields("show_clip_size", "show_fire_modes", "show_range");
                })
                .JoinService("item_category", a =>
                {
                    a.WithInjectAt("category");
                    a.OnField("item_category_id");
                })
                .JoinService("fire_mode", a =>
                {
                    a.WithInjectAt("fire_mode");
                    a.OnField("item_id");
                    a.IsList(true);
                    a.HideFields("pellets_per_shot", "pellet_spread", "max_speed", "projectile_description", "damage_type", "damage_target_type", "damage_resist_type");

                    var playerState = a.JoinService("player_state_group");
                    playerState.OnField("player_state_group_id");
                    playerState.IsList(true);
                    playerState.WithInjectAt("states");
                })
                .Where("item_id", a => a.Equals(weaponItemId.ToString()))
                .GetAsync<CensusWeaponInfoModel>();
        }
    }
}

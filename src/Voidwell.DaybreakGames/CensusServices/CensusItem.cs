using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusItem
    {
        public static async Task<IEnumerable<CensusItemModel>> GetAllItems()
        {
            var query = new CensusQuery.Query("item");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "item_id",
                "item_type_id",
                "item_category_id",
                "is_vehicle_weapon",
                "name",
                "description",
                "faction_id",
                "max_stack_size",
                "image_id"
            });

            return await query.GetBatch<CensusItemModel>();
        }

        public static async Task<JToken> GetWeaponInfo(string weaponItemId)
        {
            var query = new CensusQuery.Query("item");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "image_set_id",
                "image_path",
                "skill_set_id",
                "is_default_attachment",
                "is_vehicle_weapon",
                "passive_ability_id",
                "activatable_ability_id",
                "item_type_id"
            });

            var weapon = query.JoinService("weapon");
            weapon.WithInjectAt("weapon");
            weapon.OnField("item_id");
            weapon.ToField("weapon_id");
            weapon.HideFields(new[] {
                "weapon_id",
                "weapon_group_id"
            });

            var datasheet = query.JoinService("weapon_datasheet");
            datasheet.WithInjectAt("datasheet");
            datasheet.Where("item_id").Equals(weaponItemId);
            datasheet.HideFields(new[]
            {
                "show_clip_size",
                "show_fire_modes",
                "show_range"
            });

            var itemType = query.JoinService("item_category");
            itemType.WithInjectAt("category");
            itemType.OnField("item_category_id");

            var fireMode = query.JoinService("fire_mode");
            fireMode.WithInjectAt("fire_mode");
            fireMode.OnField("item_id");
            fireMode.IsList(true);
            fireMode.HideFields(new[]
            {
                "pellets_per_shot",
                "pellet_spread",
                "max_speed",
                "projectile_description",
                "damage_type",
                "damage_target_type",
                "damage_resist_type"
            });

            var playerState = fireMode.JoinService("player_state_group");
            playerState.OnField("player_state_group_id");
            playerState.IsList(true);
            playerState.WithInjectAt("states");

            query.Where("item_id").Equals(weaponItemId);

            return (await query.Get()).First;
        }
    }
}

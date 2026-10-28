using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class ps2dbcontext2026_10_28_13_14_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""achievement"" RENAME CONSTRAINT ""p_k_achievement"" TO ""pk_achievement"";");

            migrationBuilder.Sql(@"ALTER TABLE ""alert"" RENAME CONSTRAINT ""p_k_alert"" TO ""pk_alert"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character"" RENAME CONSTRAINT ""p_k_character"" TO ""pk_character"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_achievement"" RENAME CONSTRAINT ""p_k_character_achievement"" TO ""pk_character_achievement"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive"" RENAME CONSTRAINT ""p_k_character_directive"" TO ""pk_character_directive"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_objective"" RENAME CONSTRAINT ""p_k_character_directive_objective"" TO ""pk_character_directive_objective"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tier"" RENAME CONSTRAINT ""p_k_character_directive_tier"" TO ""pk_character_directive_tier"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tree"" RENAME CONSTRAINT ""p_k_character_directive_tree"" TO ""pk_character_directive_tree"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat"" RENAME CONSTRAINT ""p_k_character_lifetime_stat"" TO ""pk_character_lifetime_stat"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat_by_faction"" RENAME CONSTRAINT ""p_k_character_lifetime_stat_by_faction"" TO ""pk_character_lifetime_stat_by_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_rating"" RENAME CONSTRAINT ""p_k_character_rating"" TO ""pk_character_rating"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat"" RENAME CONSTRAINT ""p_k_character_stat"" TO ""pk_character_stat"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_by_faction"" RENAME CONSTRAINT ""p_k_character_stat_by_faction"" TO ""pk_character_stat_by_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_history"" RENAME CONSTRAINT ""p_k_character_stat_history"" TO ""pk_character_stat_history"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_time"" RENAME CONSTRAINT ""p_k_character_time"" TO ""pk_character_time"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_update_queue"" RENAME CONSTRAINT ""p_k_character_update_queue"" TO ""pk_character_update_queue"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat"" RENAME CONSTRAINT ""p_k_character_weapon_stat"" TO ""pk_character_weapon_stat"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat_by_faction"" RENAME CONSTRAINT ""p_k_character_weapon_stat_by_faction"" TO ""pk_character_weapon_stat_by_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""daily_population"" RENAME CONSTRAINT ""p_k_daily_population"" TO ""pk_daily_population"";");

            migrationBuilder.Sql(@"ALTER TABLE ""daily_weapon_stats"" RENAME CONSTRAINT ""p_k_daily_weapon_stats"" TO ""pk_daily_weapon_stats"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive"" RENAME CONSTRAINT ""p_k_directive"" TO ""pk_directive"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive_tier"" RENAME CONSTRAINT ""p_k_directive_tier"" TO ""pk_directive_tier"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive_tree"" RENAME CONSTRAINT ""p_k_directive_tree"" TO ""pk_directive_tree"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive_tree_category"" RENAME CONSTRAINT ""p_k_directive_tree_category"" TO ""pk_directive_tree_category"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_achievement_earned"" RENAME CONSTRAINT ""p_k_event_achievement_earned"" TO ""pk_event_achievement_earned"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_battlerank_up"" RENAME CONSTRAINT ""p_k_event_battlerank_up"" TO ""pk_event_battlerank_up"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_continent_lock"" RENAME CONSTRAINT ""p_k_event_continent_lock"" TO ""pk_event_continent_lock"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_continent_unlock"" RENAME CONSTRAINT ""p_k_event_continent_unlock"" TO ""pk_event_continent_unlock"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_death"" RENAME CONSTRAINT ""p_k_event_death"" TO ""pk_event_death"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_facility_control"" RENAME CONSTRAINT ""p_k_event_facility_control"" TO ""pk_event_facility_control"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_metagame_event"" RENAME CONSTRAINT ""p_k_event_metagame_event"" TO ""pk_event_metagame_event"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_facility_capture"" RENAME CONSTRAINT ""p_k_event_player_facility_capture"" TO ""pk_event_player_facility_capture"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_facility_defend"" RENAME CONSTRAINT ""p_k_event_player_facility_defend"" TO ""pk_event_player_facility_defend"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_login"" RENAME CONSTRAINT ""p_k_event_player_login"" TO ""pk_event_player_login"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_logout"" RENAME CONSTRAINT ""p_k_event_player_logout"" TO ""pk_event_player_logout"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_vehicle_destroy"" RENAME CONSTRAINT ""p_k_event_vehicle_destroy"" TO ""pk_event_vehicle_destroy"";");

            migrationBuilder.Sql(@"ALTER TABLE ""facility_link"" RENAME CONSTRAINT ""p_k_facility_link"" TO ""pk_facility_link"";");

            migrationBuilder.Sql(@"ALTER TABLE ""faction"" RENAME CONSTRAINT ""p_k_faction"" TO ""pk_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""image_set"" RENAME CONSTRAINT ""p_k_image_set"" TO ""pk_image_set"";");

            migrationBuilder.Sql(@"ALTER TABLE ""item"" RENAME CONSTRAINT ""p_k_item"" TO ""pk_item"";");

            migrationBuilder.Sql(@"ALTER TABLE ""item_category"" RENAME CONSTRAINT ""p_k_item_category"" TO ""pk_item_category"";");

            migrationBuilder.Sql(@"ALTER TABLE ""loadout"" RENAME CONSTRAINT ""p_k_loadout"" TO ""pk_loadout"";");

            migrationBuilder.Sql(@"ALTER TABLE ""map_hex"" RENAME CONSTRAINT ""p_k_map_hex"" TO ""pk_map_hex"";");

            migrationBuilder.Sql(@"ALTER TABLE ""map_region"" RENAME CONSTRAINT ""p_k_map_region"" TO ""pk_map_region"";");

            migrationBuilder.Sql(@"ALTER TABLE ""metagame_event_category"" RENAME CONSTRAINT ""p_k_metagame_event_category"" TO ""pk_metagame_event_category"";");

            migrationBuilder.Sql(@"ALTER TABLE ""metagame_event_category_zone"" RENAME CONSTRAINT ""p_k_metagame_event_category_zone"" TO ""pk_metagame_event_category_zone"";");

            migrationBuilder.Sql(@"ALTER TABLE ""metagame_event_state"" RENAME CONSTRAINT ""p_k_metagame_event_state"" TO ""pk_metagame_event_state"";");

            migrationBuilder.Sql(@"ALTER TABLE ""objective"" RENAME CONSTRAINT ""p_k_objective"" TO ""pk_objective"";");

            migrationBuilder.Sql(@"ALTER TABLE ""objective_set_to_objective"" RENAME CONSTRAINT ""p_k_objective_set_to_objective"" TO ""pk_objective_set_to_objective"";");

            migrationBuilder.Sql(@"ALTER TABLE ""outfit"" RENAME CONSTRAINT ""p_k_outfit"" TO ""pk_outfit"";");

            migrationBuilder.Sql(@"ALTER TABLE ""outfit_member"" RENAME CONSTRAINT ""p_k_outfit_member"" TO ""pk_outfit_member"";");

            migrationBuilder.Sql(@"ALTER TABLE ""player_session"" RENAME CONSTRAINT ""p_k_player_session"" TO ""pk_player_session"";");

            migrationBuilder.Sql(@"ALTER TABLE ""profile"" RENAME CONSTRAINT ""p_k_profile"" TO ""pk_profile"";");

            migrationBuilder.Sql(@"ALTER TABLE ""reward"" RENAME CONSTRAINT ""p_k_reward"" TO ""pk_reward"";");

            migrationBuilder.Sql(@"ALTER TABLE ""reward_group_to_reward"" RENAME CONSTRAINT ""p_k_reward_group_to_reward"" TO ""pk_reward_group_to_reward"";");

            migrationBuilder.Sql(@"ALTER TABLE ""reward_set_to_reward_group"" RENAME CONSTRAINT ""p_k_reward_set_to_reward_group"" TO ""pk_reward_set_to_reward_group"";");

            migrationBuilder.Sql(@"ALTER TABLE ""sanctioned_weapon"" RENAME CONSTRAINT ""p_k_sanctioned_weapon"" TO ""pk_sanctioned_weapon"";");

            migrationBuilder.Sql(@"ALTER TABLE ""title"" RENAME CONSTRAINT ""p_k_title"" TO ""pk_title"";");

            migrationBuilder.Sql(@"ALTER TABLE ""updater_scheduler"" RENAME CONSTRAINT ""p_k_updater_scheduler"" TO ""pk_updater_scheduler"";");

            migrationBuilder.Sql(@"ALTER TABLE ""vehicle"" RENAME CONSTRAINT ""p_k_vehicle"" TO ""pk_vehicle"";");

            migrationBuilder.Sql(@"ALTER TABLE ""vehicle_faction"" RENAME CONSTRAINT ""p_k_vehicle_faction"" TO ""pk_vehicle_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""weapon_aggregate"" RENAME CONSTRAINT ""p_k_weapon_aggregate"" TO ""pk_weapon_aggregate"";");

            migrationBuilder.Sql(@"ALTER TABLE ""world"" RENAME CONSTRAINT ""p_k_world"" TO ""pk_world"";");

            migrationBuilder.Sql(@"ALTER TABLE ""zone"" RENAME CONSTRAINT ""p_k_zone"" TO ""pk_zone"";");

            migrationBuilder.Sql(@"ALTER TABLE ""zone_ownership_snapshot"" RENAME CONSTRAINT ""p_k_zone_ownership_snapshot"" TO ""pk_zone_ownership_snapshot"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive"" RENAME CONSTRAINT ""f_k_character_directive_character_character_id"" TO ""fk_character_directive_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_objective"" RENAME CONSTRAINT ""f_k_character_directive_objective_character_character_id"" TO ""fk_character_directive_objective_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tier"" RENAME CONSTRAINT ""f_k_character_directive_tier_character_character_id"" TO ""fk_character_directive_tier_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tree"" RENAME CONSTRAINT ""f_k_character_directive_tree_character_character_id"" TO ""fk_character_directive_tree_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat"" RENAME CONSTRAINT ""f_k_character_lifetime_stat_character_character_id"" TO ""fk_character_lifetime_stat_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat_by_faction"" RENAME CONSTRAINT ""f_k_character_lifetime_stat_by_faction_character_character_id"" TO ""fk_character_lifetime_stat_by_faction_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_rating"" RENAME CONSTRAINT ""f_k_character_rating_character_character_id"" TO ""fk_character_rating_characters_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat"" RENAME CONSTRAINT ""f_k_character_stat_character_character_id"" TO ""fk_character_stat_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_by_faction"" RENAME CONSTRAINT ""f_k_character_stat_by_faction_character_character_id"" TO ""fk_character_stat_by_faction_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_history"" RENAME CONSTRAINT ""f_k_character_stat_history_character_character_id"" TO ""fk_character_stat_history_characters_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_time"" RENAME CONSTRAINT ""f_k_character_time_character_character_id"" TO ""fk_character_time_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat"" RENAME CONSTRAINT ""f_k_character_weapon_stat_character_character_id"" TO ""fk_character_weapon_stat_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat_by_faction"" RENAME CONSTRAINT ""f_k_character_weapon_stat_by_faction_character_character_id"" TO ""fk_character_weapon_stat_by_faction_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""outfit_member"" RENAME CONSTRAINT ""f_k_outfit_member_character_character_id"" TO ""fk_outfit_member_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""vehicle_faction"" RENAME CONSTRAINT ""f_k_vehicle_faction_vehicle_vehicle_id"" TO ""fk_vehicle_faction_vehicle_vehicle_id"";");

            migrationBuilder.RenameIndex(
                name: "i_x_zone_ownership_snapshot_world_id_metagame_instance_id",
                table: "zone_ownership_snapshot",
                newName: "ix_zone_ownership_snapshot_world_id_metagame_instance_id");

            migrationBuilder.RenameIndex(
                name: "i_x_player_session_character_id_login_date_logout_date",
                table: "player_session",
                newName: "ix_player_session_character_id_login_date_logout_date");

            migrationBuilder.RenameIndex(
                name: "i_x_event_player_logout_timestamp_world_id",
                table: "event_player_logout",
                newName: "ix_event_player_logout_timestamp_world_id");

            migrationBuilder.RenameIndex(
                name: "i_x_event_player_login_timestamp_world_id",
                table: "event_player_login",
                newName: "ix_event_player_login_timestamp_world_id");

            migrationBuilder.RenameIndex(
                name: "i_x_character_weapon_stat_item_id_kills",
                table: "character_weapon_stat",
                newName: "ix_character_weapon_stat_item_id_kills");

            migrationBuilder.RenameIndex(
                name: "i_x_character_rating_rating",
                table: "character_rating",
                newName: "ix_character_rating_rating");

            migrationBuilder.RenameIndex(
                name: "i_x_character_name",
                table: "character",
                newName: "ix_character_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""achievement"" RENAME CONSTRAINT ""pk_achievement"" TO ""p_k_achievement"";");

            migrationBuilder.Sql(@"ALTER TABLE ""alert"" RENAME CONSTRAINT ""pk_alert"" TO ""p_k_alert"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character"" RENAME CONSTRAINT ""pk_character"" TO ""p_k_character"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_achievement"" RENAME CONSTRAINT ""pk_character_achievement"" TO ""p_k_character_achievement"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive"" RENAME CONSTRAINT ""pk_character_directive"" TO ""p_k_character_directive"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_objective"" RENAME CONSTRAINT ""pk_character_directive_objective"" TO ""p_k_character_directive_objective"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tier"" RENAME CONSTRAINT ""pk_character_directive_tier"" TO ""p_k_character_directive_tier"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tree"" RENAME CONSTRAINT ""pk_character_directive_tree"" TO ""p_k_character_directive_tree"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat"" RENAME CONSTRAINT ""pk_character_lifetime_stat"" TO ""p_k_character_lifetime_stat"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat_by_faction"" RENAME CONSTRAINT ""pk_character_lifetime_stat_by_faction"" TO ""p_k_character_lifetime_stat_by_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_rating"" RENAME CONSTRAINT ""pk_character_rating"" TO ""p_k_character_rating"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat"" RENAME CONSTRAINT ""pk_character_stat"" TO ""p_k_character_stat"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_by_faction"" RENAME CONSTRAINT ""pk_character_stat_by_faction"" TO ""p_k_character_stat_by_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_history"" RENAME CONSTRAINT ""pk_character_stat_history"" TO ""p_k_character_stat_history"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_time"" RENAME CONSTRAINT ""pk_character_time"" TO ""p_k_character_time"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_update_queue"" RENAME CONSTRAINT ""pk_character_update_queue"" TO ""p_k_character_update_queue"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat"" RENAME CONSTRAINT ""pk_character_weapon_stat"" TO ""p_k_character_weapon_stat"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat_by_faction"" RENAME CONSTRAINT ""pk_character_weapon_stat_by_faction"" TO ""p_k_character_weapon_stat_by_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""daily_population"" RENAME CONSTRAINT ""pk_daily_population"" TO ""p_k_daily_population"";");

            migrationBuilder.Sql(@"ALTER TABLE ""daily_weapon_stats"" RENAME CONSTRAINT ""pk_daily_weapon_stats"" TO ""p_k_daily_weapon_stats"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive"" RENAME CONSTRAINT ""pk_directive"" TO ""p_k_directive"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive_tier"" RENAME CONSTRAINT ""pk_directive_tier"" TO ""p_k_directive_tier"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive_tree"" RENAME CONSTRAINT ""pk_directive_tree"" TO ""p_k_directive_tree"";");

            migrationBuilder.Sql(@"ALTER TABLE ""directive_tree_category"" RENAME CONSTRAINT ""pk_directive_tree_category"" TO ""p_k_directive_tree_category"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_achievement_earned"" RENAME CONSTRAINT ""pk_event_achievement_earned"" TO ""p_k_event_achievement_earned"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_battlerank_up"" RENAME CONSTRAINT ""pk_event_battlerank_up"" TO ""p_k_event_battlerank_up"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_continent_lock"" RENAME CONSTRAINT ""pk_event_continent_lock"" TO ""p_k_event_continent_lock"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_continent_unlock"" RENAME CONSTRAINT ""pk_event_continent_unlock"" TO ""p_k_event_continent_unlock"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_death"" RENAME CONSTRAINT ""pk_event_death"" TO ""p_k_event_death"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_facility_control"" RENAME CONSTRAINT ""pk_event_facility_control"" TO ""p_k_event_facility_control"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_metagame_event"" RENAME CONSTRAINT ""pk_event_metagame_event"" TO ""p_k_event_metagame_event"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_facility_capture"" RENAME CONSTRAINT ""pk_event_player_facility_capture"" TO ""p_k_event_player_facility_capture"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_facility_defend"" RENAME CONSTRAINT ""pk_event_player_facility_defend"" TO ""p_k_event_player_facility_defend"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_login"" RENAME CONSTRAINT ""pk_event_player_login"" TO ""p_k_event_player_login"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_player_logout"" RENAME CONSTRAINT ""pk_event_player_logout"" TO ""p_k_event_player_logout"";");

            migrationBuilder.Sql(@"ALTER TABLE ""event_vehicle_destroy"" RENAME CONSTRAINT ""pk_event_vehicle_destroy"" TO ""p_k_event_vehicle_destroy"";");

            migrationBuilder.Sql(@"ALTER TABLE ""facility_link"" RENAME CONSTRAINT ""pk_facility_link"" TO ""p_k_facility_link"";");

            migrationBuilder.Sql(@"ALTER TABLE ""faction"" RENAME CONSTRAINT ""pk_faction"" TO ""p_k_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""image_set"" RENAME CONSTRAINT ""pk_image_set"" TO ""p_k_image_set"";");

            migrationBuilder.Sql(@"ALTER TABLE ""item"" RENAME CONSTRAINT ""pk_item"" TO ""p_k_item"";");

            migrationBuilder.Sql(@"ALTER TABLE ""item_category"" RENAME CONSTRAINT ""pk_item_category"" TO ""p_k_item_category"";");

            migrationBuilder.Sql(@"ALTER TABLE ""loadout"" RENAME CONSTRAINT ""pk_loadout"" TO ""p_k_loadout"";");

            migrationBuilder.Sql(@"ALTER TABLE ""map_hex"" RENAME CONSTRAINT ""pk_map_hex"" TO ""p_k_map_hex"";");

            migrationBuilder.Sql(@"ALTER TABLE ""map_region"" RENAME CONSTRAINT ""pk_map_region"" TO ""p_k_map_region"";");

            migrationBuilder.Sql(@"ALTER TABLE ""metagame_event_category"" RENAME CONSTRAINT ""pk_metagame_event_category"" TO ""p_k_metagame_event_category"";");

            migrationBuilder.Sql(@"ALTER TABLE ""metagame_event_category_zone"" RENAME CONSTRAINT ""pk_metagame_event_category_zone"" TO ""p_k_metagame_event_category_zone"";");

            migrationBuilder.Sql(@"ALTER TABLE ""metagame_event_state"" RENAME CONSTRAINT ""pk_metagame_event_state"" TO ""p_k_metagame_event_state"";");

            migrationBuilder.Sql(@"ALTER TABLE ""objective"" RENAME CONSTRAINT ""pk_objective"" TO ""p_k_objective"";");

            migrationBuilder.Sql(@"ALTER TABLE ""objective_set_to_objective"" RENAME CONSTRAINT ""pk_objective_set_to_objective"" TO ""p_k_objective_set_to_objective"";");

            migrationBuilder.Sql(@"ALTER TABLE ""outfit"" RENAME CONSTRAINT ""pk_outfit"" TO ""p_k_outfit"";");

            migrationBuilder.Sql(@"ALTER TABLE ""outfit_member"" RENAME CONSTRAINT ""pk_outfit_member"" TO ""p_k_outfit_member"";");

            migrationBuilder.Sql(@"ALTER TABLE ""player_session"" RENAME CONSTRAINT ""pk_player_session"" TO ""p_k_player_session"";");

            migrationBuilder.Sql(@"ALTER TABLE ""profile"" RENAME CONSTRAINT ""pk_profile"" TO ""p_k_profile"";");

            migrationBuilder.Sql(@"ALTER TABLE ""reward"" RENAME CONSTRAINT ""pk_reward"" TO ""p_k_reward"";");

            migrationBuilder.Sql(@"ALTER TABLE ""reward_group_to_reward"" RENAME CONSTRAINT ""pk_reward_group_to_reward"" TO ""p_k_reward_group_to_reward"";");

            migrationBuilder.Sql(@"ALTER TABLE ""reward_set_to_reward_group"" RENAME CONSTRAINT ""pk_reward_set_to_reward_group"" TO ""p_k_reward_set_to_reward_group"";");

            migrationBuilder.Sql(@"ALTER TABLE ""sanctioned_weapon"" RENAME CONSTRAINT ""pk_sanctioned_weapon"" TO ""p_k_sanctioned_weapon"";");

            migrationBuilder.Sql(@"ALTER TABLE ""title"" RENAME CONSTRAINT ""pk_title"" TO ""p_k_title"";");

            migrationBuilder.Sql(@"ALTER TABLE ""updater_scheduler"" RENAME CONSTRAINT ""pk_updater_scheduler"" TO ""p_k_updater_scheduler"";");

            migrationBuilder.Sql(@"ALTER TABLE ""vehicle"" RENAME CONSTRAINT ""pk_vehicle"" TO ""p_k_vehicle"";");

            migrationBuilder.Sql(@"ALTER TABLE ""vehicle_faction"" RENAME CONSTRAINT ""pk_vehicle_faction"" TO ""p_k_vehicle_faction"";");

            migrationBuilder.Sql(@"ALTER TABLE ""weapon_aggregate"" RENAME CONSTRAINT ""pk_weapon_aggregate"" TO ""p_k_weapon_aggregate"";");

            migrationBuilder.Sql(@"ALTER TABLE ""world"" RENAME CONSTRAINT ""pk_world"" TO ""p_k_world"";");

            migrationBuilder.Sql(@"ALTER TABLE ""zone"" RENAME CONSTRAINT ""pk_zone"" TO ""p_k_zone"";");

            migrationBuilder.Sql(@"ALTER TABLE ""zone_ownership_snapshot"" RENAME CONSTRAINT ""pk_zone_ownership_snapshot"" TO ""p_k_zone_ownership_snapshot"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive"" RENAME CONSTRAINT ""fk_character_directive_character_character_id"" TO ""f_k_character_directive_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_objective"" RENAME CONSTRAINT ""fk_character_directive_objective_character_character_id"" TO ""f_k_character_directive_objective_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tier"" RENAME CONSTRAINT ""fk_character_directive_tier_character_character_id"" TO ""f_k_character_directive_tier_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_directive_tree"" RENAME CONSTRAINT ""fk_character_directive_tree_character_character_id"" TO ""f_k_character_directive_tree_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat"" RENAME CONSTRAINT ""fk_character_lifetime_stat_character_character_id"" TO ""f_k_character_lifetime_stat_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_lifetime_stat_by_faction"" RENAME CONSTRAINT ""fk_character_lifetime_stat_by_faction_character_character_id"" TO ""f_k_character_lifetime_stat_by_faction_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_rating"" RENAME CONSTRAINT ""fk_character_rating_characters_character_id"" TO ""f_k_character_rating_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat"" RENAME CONSTRAINT ""fk_character_stat_character_character_id"" TO ""f_k_character_stat_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_by_faction"" RENAME CONSTRAINT ""fk_character_stat_by_faction_character_character_id"" TO ""f_k_character_stat_by_faction_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_stat_history"" RENAME CONSTRAINT ""fk_character_stat_history_characters_character_id"" TO ""f_k_character_stat_history_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_time"" RENAME CONSTRAINT ""fk_character_time_character_character_id"" TO ""f_k_character_time_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat"" RENAME CONSTRAINT ""fk_character_weapon_stat_character_character_id"" TO ""f_k_character_weapon_stat_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""character_weapon_stat_by_faction"" RENAME CONSTRAINT ""fk_character_weapon_stat_by_faction_character_character_id"" TO ""f_k_character_weapon_stat_by_faction_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""outfit_member"" RENAME CONSTRAINT ""fk_outfit_member_character_character_id"" TO ""f_k_outfit_member_character_character_id"";");

            migrationBuilder.Sql(@"ALTER TABLE ""vehicle_faction"" RENAME CONSTRAINT ""fk_vehicle_faction_vehicle_vehicle_id"" TO ""f_k_vehicle_faction_vehicle_vehicle_id"";");

            migrationBuilder.RenameIndex(
                name: "ix_zone_ownership_snapshot_world_id_metagame_instance_id",
                table: "zone_ownership_snapshot",
                newName: "i_x_zone_ownership_snapshot_world_id_metagame_instance_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_session_character_id_login_date_logout_date",
                table: "player_session",
                newName: "i_x_player_session_character_id_login_date_logout_date");

            migrationBuilder.RenameIndex(
                name: "ix_event_player_logout_timestamp_world_id",
                table: "event_player_logout",
                newName: "i_x_event_player_logout_timestamp_world_id");

            migrationBuilder.RenameIndex(
                name: "ix_event_player_login_timestamp_world_id",
                table: "event_player_login",
                newName: "i_x_event_player_login_timestamp_world_id");

            migrationBuilder.RenameIndex(
                name: "ix_character_weapon_stat_item_id_kills",
                table: "character_weapon_stat",
                newName: "i_x_character_weapon_stat_item_id_kills");

            migrationBuilder.RenameIndex(
                name: "ix_character_rating_rating",
                table: "character_rating",
                newName: "i_x_character_rating_rating");

            migrationBuilder.RenameIndex(
                name: "ix_character_name",
                table: "character",
                newName: "i_x_character_name");
        }
    }
}

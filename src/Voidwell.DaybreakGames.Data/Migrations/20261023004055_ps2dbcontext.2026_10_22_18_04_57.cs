using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class ps2dbcontext2026_10_22_18_04_57 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_gain_experience");

            migrationBuilder.DropTable(
                name: "experience");

            migrationBuilder.Sql("DELETE FROM updater_scheduler WHERE id = 'ExperienceStore';");

            migrationBuilder.DropIndex(
                name: "i_x_event_vehicle_destroy_attacker_weapon_id_timestamp",
                table: "event_vehicle_destroy");

            migrationBuilder.DropIndex(
                name: "i_x_event_death_attacker_weapon_id_timestamp",
                table: "event_death");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_character_character_id",
                table: "character_directive");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_objective_character_character_id",
                table: "character_directive_objective");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_tier_character_character_id",
                table: "character_directive_tier");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_tree_character_character_id",
                table: "character_directive_tree");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_lifetime_stat_character_character_id",
                table: "character_lifetime_stat");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_lifetime_stat_by_faction_character_character_id",
                table: "character_lifetime_stat_by_faction");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_rating_character_character_id",
                table: "character_rating");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_stat_character_character_id",
                table: "character_stat");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_stat_by_faction_character_character_id",
                table: "character_stat_by_faction");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_stat_history_character_character_id",
                table: "character_stat_history");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_time_character_character_id",
                table: "character_time");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_weapon_stat_character_character_id",
                table: "character_weapon_stat");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_weapon_stat_by_faction_character_character_id",
                table: "character_weapon_stat_by_faction");

            migrationBuilder.DropForeignKey(
                name: "f_k_outfit_member_character_character_id",
                table: "outfit_member");

            migrationBuilder.Sql("ALTER TABLE \"player_session\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"outfit_member\" ALTER COLUMN outfit_id TYPE bigint USING outfit_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"outfit_member\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"outfit\" ALTER COLUMN leader_character_id TYPE bigint USING NULLIF(leader_character_id, '')::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"outfit\" ALTER COLUMN id TYPE bigint USING id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_vehicle_destroy\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_vehicle_destroy\" ALTER COLUMN attacker_character_id TYPE bigint USING attacker_character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_player_logout\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_player_login\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_player_facility_defend\" ALTER COLUMN outfit_id TYPE bigint USING NULLIF(outfit_id, '')::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_player_facility_defend\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_player_facility_capture\" ALTER COLUMN outfit_id TYPE bigint USING NULLIF(outfit_id, '')::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_player_facility_capture\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_facility_control\" ALTER COLUMN outfit_id TYPE bigint USING NULLIF(outfit_id, '')::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_death\" ALTER COLUMN character_outfit_id TYPE bigint USING NULLIF(character_outfit_id, '')::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_death\" ALTER COLUMN attacker_outfit_id TYPE bigint USING NULLIF(attacker_outfit_id, '')::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_death\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_death\" ALTER COLUMN attacker_character_id TYPE bigint USING attacker_character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_battlerank_up\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"event_achievement_earned\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_weapon_stat_by_faction\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_weapon_stat\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_update_queue\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_time\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_stat_history\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_stat_by_faction\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_stat\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_rating\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_lifetime_stat_by_faction\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_lifetime_stat\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_directive_tree\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_directive_tier\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_directive_objective\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_directive\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character_achievement\" ALTER COLUMN character_id TYPE bigint USING character_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE \"character\" ALTER COLUMN id TYPE bigint USING id::bigint;");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "player_session",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "outfit_id",
                table: "outfit_member",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "outfit_member",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "leader_character_id",
                table: "outfit",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "outfit",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_vehicle_destroy",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "attacker_character_id",
                table: "event_vehicle_destroy",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_player_logout",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_player_login",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "outfit_id",
                table: "event_player_facility_defend",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_player_facility_defend",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "outfit_id",
                table: "event_player_facility_capture",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_player_facility_capture",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "outfit_id",
                table: "event_facility_control",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "character_outfit_id",
                table: "event_death",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "attacker_outfit_id",
                table: "event_death",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_death",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "attacker_character_id",
                table: "event_death",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_battlerank_up",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "event_achievement_earned",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_weapon_stat_by_faction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_weapon_stat",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_update_queue",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_time",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_stat_history",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_stat_by_faction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_stat",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_rating",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_lifetime_stat_by_faction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_lifetime_stat",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_directive_tree",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_directive_tier",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_directive_objective",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_directive",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "character_id",
                table: "character_achievement",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "character",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_character_character_id",
                table: "character_directive",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_objective_character_character_id",
                table: "character_directive_objective",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_tier_character_character_id",
                table: "character_directive_tier",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_tree_character_character_id",
                table: "character_directive_tree",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_lifetime_stat_character_character_id",
                table: "character_lifetime_stat",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_lifetime_stat_by_faction_character_character_id",
                table: "character_lifetime_stat_by_faction",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_rating_character_character_id",
                table: "character_rating",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_stat_character_character_id",
                table: "character_stat",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_stat_by_faction_character_character_id",
                table: "character_stat_by_faction",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_stat_history_character_character_id",
                table: "character_stat_history",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_time_character_character_id",
                table: "character_time",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_weapon_stat_character_character_id",
                table: "character_weapon_stat",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_weapon_stat_by_faction_character_character_id",
                table: "character_weapon_stat_by_faction",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_outfit_member_character_character_id",
                table: "outfit_member",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_character_character_id",
                table: "character_directive");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_objective_character_character_id",
                table: "character_directive_objective");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_tier_character_character_id",
                table: "character_directive_tier");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_tree_character_character_id",
                table: "character_directive_tree");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_lifetime_stat_character_character_id",
                table: "character_lifetime_stat");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_lifetime_stat_by_faction_character_character_id",
                table: "character_lifetime_stat_by_faction");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_rating_character_character_id",
                table: "character_rating");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_stat_character_character_id",
                table: "character_stat");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_stat_by_faction_character_character_id",
                table: "character_stat_by_faction");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_stat_history_character_character_id",
                table: "character_stat_history");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_time_character_character_id",
                table: "character_time");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_weapon_stat_character_character_id",
                table: "character_weapon_stat");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_weapon_stat_by_faction_character_character_id",
                table: "character_weapon_stat_by_faction");

            migrationBuilder.DropForeignKey(
                name: "f_k_outfit_member_character_character_id",
                table: "outfit_member");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "player_session",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "outfit_id",
                table: "outfit_member",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "outfit_member",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "leader_character_id",
                table: "outfit",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "outfit",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_vehicle_destroy",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "attacker_character_id",
                table: "event_vehicle_destroy",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_player_logout",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_player_login",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "outfit_id",
                table: "event_player_facility_defend",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_player_facility_defend",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "outfit_id",
                table: "event_player_facility_capture",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_player_facility_capture",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "outfit_id",
                table: "event_facility_control",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "character_outfit_id",
                table: "event_death",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "attacker_outfit_id",
                table: "event_death",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_death",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "attacker_character_id",
                table: "event_death",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_battlerank_up",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "event_achievement_earned",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_weapon_stat_by_faction",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_weapon_stat",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_update_queue",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_time",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_stat_history",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_stat_by_faction",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_stat",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_rating",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_lifetime_stat_by_faction",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_lifetime_stat",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_directive_tree",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_directive_tier",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_directive_objective",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_directive",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "character_id",
                table: "character_achievement",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "character",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_character_character_id",
                table: "character_directive",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_objective_character_character_id",
                table: "character_directive_objective",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_tier_character_character_id",
                table: "character_directive_tier",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_tree_character_character_id",
                table: "character_directive_tree",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_lifetime_stat_character_character_id",
                table: "character_lifetime_stat",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_lifetime_stat_by_faction_character_character_id",
                table: "character_lifetime_stat_by_faction",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_rating_character_character_id",
                table: "character_rating",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_stat_character_character_id",
                table: "character_stat",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_stat_by_faction_character_character_id",
                table: "character_stat_by_faction",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_stat_history_character_character_id",
                table: "character_stat_history",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_time_character_character_id",
                table: "character_time",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_weapon_stat_character_character_id",
                table: "character_weapon_stat",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_weapon_stat_by_faction_character_character_id",
                table: "character_weapon_stat_by_faction",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_outfit_member_character_character_id",
                table: "outfit_member",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateTable(
                name: "event_gain_experience",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    character_id = table.Column<string>(type: "text", nullable: false),
                    experience_id = table.Column<int>(type: "integer", nullable: false),
                    loadout_id = table.Column<int>(type: "integer", nullable: true),
                    other_id = table.Column<string>(type: "text", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    world_id = table.Column<int>(type: "integer", nullable: false),
                    zone_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_gain_experience", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "experience",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    xp = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_experience", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_event_vehicle_destroy_attacker_weapon_id_timestamp",
                table: "event_vehicle_destroy",
                columns: new[] { "attacker_weapon_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_death_attacker_weapon_id_timestamp",
                table: "event_death",
                columns: new[] { "attacker_weapon_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_timestamp_character_id_experience_id",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "character_id", "experience_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_timestamp_world_id_experience_id_zone",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "world_id", "experience_id", "zone_id" });
        }
    }
}

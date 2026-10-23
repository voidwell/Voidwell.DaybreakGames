using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class ps2dbcontext2026_10_23_01_28_12 : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

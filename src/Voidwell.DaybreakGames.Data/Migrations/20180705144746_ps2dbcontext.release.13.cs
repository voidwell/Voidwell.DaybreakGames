using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_event_death_attacker_weapon_id",
                table: "event_death");

            migrationBuilder.CreateTable(
                name: "daily_weapon_stats",
                columns: table => new
                {
                    stat_name = table.Column<string>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_daily_weapon_stats", x => new { x.stat_name, x.date });
                });

            migrationBuilder.CreateIndex(
                name: "i_x_event_death_attacker_weapon_id_timestamp",
                table: "event_death",
                columns: new[] { "attacker_weapon_id", "timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_weapon_stats");

            migrationBuilder.DropIndex(
                name: "i_x_event_death_attacker_weapon_id_timestamp",
                table: "event_death");

            migrationBuilder.CreateIndex(
                name: "i_x_event_death_attacker_weapon_id",
                table: "event_death",
                column: "attacker_weapon_id");
        }
    }
}

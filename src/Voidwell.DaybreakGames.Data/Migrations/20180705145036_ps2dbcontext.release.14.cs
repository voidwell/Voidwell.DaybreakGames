using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats");

            migrationBuilder.AddColumn<int>(
                name: "weapon_id",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats",
                columns: new[] { "stat_name", "weapon_id", "date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "weapon_id",
                table: "daily_weapon_stats");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats",
                columns: new[] { "stat_name", "date" });
        }
    }
}

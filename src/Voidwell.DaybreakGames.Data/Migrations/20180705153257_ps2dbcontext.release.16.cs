using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "stat_name",
                table: "daily_weapon_stats");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats",
                columns: new[] { "weapon_id", "date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats");

            migrationBuilder.AddColumn<string>(
                name: "stat_name",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_daily_weapon_stats",
                table: "daily_weapon_stats",
                columns: new[] { "stat_name", "weapon_id", "date" });
        }
    }
}

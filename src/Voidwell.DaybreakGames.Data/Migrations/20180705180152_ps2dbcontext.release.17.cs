using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "headshots",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "q4_headshots",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "q4_kills",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "q4_uniques",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "headshots",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q4_headshots",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q4_kills",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q4_uniques",
                table: "daily_weapon_stats");
        }
    }
}

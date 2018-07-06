using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "daily_weapon_stats",
                newName: "uniques");

            migrationBuilder.AddColumn<float>(
                name: "avg_br",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "kills",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "q1_kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "q2_kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "q3_kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "q4_kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avg_br",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "kills",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "kpu",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q1_kpu",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q2_kpu",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q3_kpu",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "q4_kpu",
                table: "daily_weapon_stats");

            migrationBuilder.RenameColumn(
                name: "uniques",
                table: "daily_weapon_stats",
                newName: "value");
        }
    }
}

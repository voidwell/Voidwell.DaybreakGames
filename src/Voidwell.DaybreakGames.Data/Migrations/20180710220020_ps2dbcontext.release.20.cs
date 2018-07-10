using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "aircraft_kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "vehicle_kpu",
                table: "daily_weapon_stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "i_x_event_vehicle_destroy_attacker_weapon_id_timestamp",
                table: "event_vehicle_destroy",
                columns: new[] { "attacker_weapon_id", "timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_event_vehicle_destroy_attacker_weapon_id_timestamp",
                table: "event_vehicle_destroy");

            migrationBuilder.DropColumn(
                name: "aircraft_kpu",
                table: "daily_weapon_stats");

            migrationBuilder.DropColumn(
                name: "vehicle_kpu",
                table: "daily_weapon_stats");
        }
    }
}

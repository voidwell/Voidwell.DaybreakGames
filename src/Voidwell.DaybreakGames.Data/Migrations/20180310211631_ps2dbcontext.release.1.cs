using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_facility_control",
                table: "event_facility_control");

            migrationBuilder.AlterColumn<int>(
                name: "new_faction_id",
                table: "event_facility_control",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_facility_control",
                table: "event_facility_control",
                columns: new[] { "timestamp", "world_id", "facility_id", "new_faction_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_facility_control",
                table: "event_facility_control");

            migrationBuilder.AlterColumn<int>(
                name: "new_faction_id",
                table: "event_facility_control",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_facility_control",
                table: "event_facility_control",
                columns: new[] { "timestamp", "world_id", "facility_id" });
        }
    }
}

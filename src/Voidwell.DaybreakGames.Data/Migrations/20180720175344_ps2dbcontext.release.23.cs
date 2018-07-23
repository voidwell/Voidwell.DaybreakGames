using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "zone_population_nc",
                table: "event_facility_control",
                type: "int4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "zone_population_tr",
                table: "event_facility_control",
                type: "int4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "zone_population_vs",
                table: "event_facility_control",
                type: "int4",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "zone_population_nc",
                table: "event_facility_control");

            migrationBuilder.DropColumn(
                name: "zone_population_tr",
                table: "event_facility_control");

            migrationBuilder.DropColumn(
                name: "zone_population_vs",
                table: "event_facility_control");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_map_region",
                table: "map_region");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_map_region",
                table: "map_region",
                columns: new[] { "id", "facility_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_map_region",
                table: "map_region");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_map_region",
                table: "map_region",
                column: "id");
        }
    }
}

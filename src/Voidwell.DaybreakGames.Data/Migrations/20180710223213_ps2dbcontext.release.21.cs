using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_zone_ownership_snapshot",
                table: "zone_ownership_snapshot");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_zone_ownership_snapshot",
                table: "zone_ownership_snapshot",
                columns: new[] { "timestamp", "world_id", "zone_id", "region_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_zone_ownership_snapshot",
                table: "zone_ownership_snapshot");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_zone_ownership_snapshot",
                table: "zone_ownership_snapshot",
                columns: new[] { "timestamp", "world_id", "zone_id" });
        }
    }
}

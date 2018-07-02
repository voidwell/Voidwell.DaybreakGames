using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "zone_ownership_snapshot",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false),
                    faction_id = table.Column<int>(nullable: false),
                    metagame_instance_id = table.Column<int>(nullable: true),
                    region_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_zone_ownership_snapshot", x => new { x.timestamp, x.world_id, x.zone_id });
                });

            migrationBuilder.CreateIndex(
                name: "i_x_zone_ownership_snapshot_world_id_metagame_instance_id",
                table: "zone_ownership_snapshot",
                columns: new[] { "world_id", "metagame_instance_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "zone_ownership_snapshot");
        }
    }
}

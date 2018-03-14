using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metagame_event_category_zone",
                columns: table => new
                {
                    metagame_event_category_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_metagame_event_category_zone", x => x.metagame_event_category_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metagame_event_category_zone");
        }
    }
}

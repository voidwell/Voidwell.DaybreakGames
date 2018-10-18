using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_population",
                columns: table => new
                {
                    date = table.Column<DateTime>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    vs_count = table.Column<int>(nullable: false),
                    nc_count = table.Column<int>(nullable: false),
                    tr_count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_daily_population", x => new { x.date, x.world_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_population");
        }
    }
}

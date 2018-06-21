using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_stat_history",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    all_time = table.Column<int>(nullable: false),
                    day = table.Column<string>(nullable: true),
                    month = table.Column<string>(nullable: true),
                    one_life_max = table.Column<int>(nullable: false),
                    stat_name = table.Column<string>(nullable: true),
                    week = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_stat_history", x => x.character_id);
                    table.ForeignKey(
                        name: "f_k_character_stat_history_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_stat_history");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_character_weapon_stat_kills",
                table: "character_weapon_stat");

            migrationBuilder.CreateIndex(
                name: "i_x_character_weapon_stat_item_id_kills",
                table: "character_weapon_stat",
                columns: new[] { "item_id", "kills" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_character_weapon_stat_item_id_kills",
                table: "character_weapon_stat");

            migrationBuilder.CreateIndex(
                name: "i_x_character_weapon_stat_kills",
                table: "character_weapon_stat",
                column: "kills");
        }
    }
}

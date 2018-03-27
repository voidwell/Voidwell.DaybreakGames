using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "i_x_character_name",
                table: "character",
                column: "name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_character_name",
                table: "character");
        }
    }
}

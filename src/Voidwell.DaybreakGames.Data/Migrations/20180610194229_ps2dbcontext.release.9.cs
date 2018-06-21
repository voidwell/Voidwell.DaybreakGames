using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_character_stat_history",
                table: "character_stat_history");

            migrationBuilder.AlterColumn<string>(
                name: "stat_name",
                table: "character_stat_history",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_character_stat_history",
                table: "character_stat_history",
                columns: new[] { "character_id", "stat_name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_character_stat_history",
                table: "character_stat_history");

            migrationBuilder.AlterColumn<string>(
                name: "stat_name",
                table: "character_stat_history",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "p_k_character_stat_history",
                table: "character_stat_history",
                column: "character_id");
        }
    }
}

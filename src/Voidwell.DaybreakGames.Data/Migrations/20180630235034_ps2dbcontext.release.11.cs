using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_map_hex",
                table: "map_hex");

            migrationBuilder.DropColumn(
                name: "id",
                table: "map_hex");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_map_hex",
                table: "map_hex",
                columns: new[] { "map_region_id", "x_pos", "y_pos", "zone_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_map_hex",
                table: "map_hex");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "map_hex",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_map_hex",
                table: "map_hex",
                column: "id");
        }
    }
}

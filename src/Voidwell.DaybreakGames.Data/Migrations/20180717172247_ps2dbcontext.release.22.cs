using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience");

            migrationBuilder.AlterColumn<string>(
                name: "other_id",
                table: "event_gain_experience",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "character_id", "experience_id", "other_id" });

            migrationBuilder.CreateTable(
                name: "experience",
                columns: table => new
                {
                    id = table.Column<int>(type: "int4", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    xp = table.Column<float>(type: "float4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_experience", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_world_id_zone_id_timestamp",
                table: "event_gain_experience",
                columns: new[] { "world_id", "zone_id", "timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "experience");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience");

            migrationBuilder.DropIndex(
                name: "i_x_event_gain_experience_world_id_zone_id_timestamp",
                table: "event_gain_experience");

            migrationBuilder.AlterColumn<string>(
                name: "other_id",
                table: "event_gain_experience",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "character_id", "experience_id" });
        }
    }
}

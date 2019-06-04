using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "event_gain_experience",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience",
                columns: new[] { "id", "timestamp", "character_id", "experience_id", "other_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience");

            migrationBuilder.DropColumn(
                name: "id",
                table: "event_gain_experience");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "character_id", "experience_id", "other_id" });
        }
    }
}

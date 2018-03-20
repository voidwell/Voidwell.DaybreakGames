using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_battlerank_up",
                table: "event_battlerank_up");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_battlerank_up",
                table: "event_battlerank_up",
                columns: new[] { "character_id", "timestamp", "battle_rank" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_battlerank_up",
                table: "event_battlerank_up");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_battlerank_up",
                table: "event_battlerank_up",
                columns: new[] { "character_id", "timestamp" });
        }
    }
}

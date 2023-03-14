using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontext2023_03_07_15_18_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_character_directive_tree_character_id_dir",
                table: "character_directive");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_objective_character_directive_character",
                table: "character_directive_objective");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_tier_character_directive_tree_character",
                table: "character_directive_tier");

            migrationBuilder.DropIndex(
                name: "i_x_character_directive_character_id_directive_tree_id",
                table: "character_directive");

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_character_character_id",
                table: "character_directive",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_objective_character_character_id",
                table: "character_directive_objective",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_tier_character_character_id",
                table: "character_directive_tier",
                column: "character_id",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_character_character_id",
                table: "character_directive");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_objective_character_character_id",
                table: "character_directive_objective");

            migrationBuilder.DropForeignKey(
                name: "f_k_character_directive_tier_character_character_id",
                table: "character_directive_tier");

            migrationBuilder.CreateIndex(
                name: "i_x_character_directive_character_id_directive_tree_id",
                table: "character_directive",
                columns: new[] { "character_id", "directive_tree_id" });

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_character_directive_tree_character_id_dir",
                table: "character_directive",
                columns: new[] { "character_id", "directive_tree_id" },
                principalTable: "character_directive_tree",
                principalColumns: new[] { "character_id", "directive_tree_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_objective_character_directive_character",
                table: "character_directive_objective",
                columns: new[] { "character_id", "directive_id" },
                principalTable: "character_directive",
                principalColumns: new[] { "character_id", "directive_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_character_directive_tier_character_directive_tree_character",
                table: "character_directive_tier",
                columns: new[] { "character_id", "directive_tree_id" },
                principalTable: "character_directive_tree",
                principalColumns: new[] { "character_id", "directive_tree_id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

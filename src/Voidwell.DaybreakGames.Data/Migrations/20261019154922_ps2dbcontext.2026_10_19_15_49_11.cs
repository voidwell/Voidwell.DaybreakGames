using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class ps2dbcontext2026_10_19_15_49_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "world",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            // Every world other than the current live servers is legacy.
            migrationBuilder.Sql("UPDATE world SET is_active = false WHERE id NOT IN (1, 10, 19, 40, 1000, 2000);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "world");
        }
    }
}

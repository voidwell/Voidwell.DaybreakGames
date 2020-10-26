using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_map_region",
                table: "map_region");

            migrationBuilder.AlterColumn<int>(
                name: "facility_id",
                table: "map_region",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "p_k_map_region",
                table: "map_region",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_map_region",
                table: "map_region");

            migrationBuilder.AlterColumn<int>(
                name: "facility_id",
                table: "map_region",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_map_region",
                table: "map_region",
                columns: new[] { "id", "facility_id" });
        }
    }
}

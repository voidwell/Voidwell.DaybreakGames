using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease35 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_facility_link",
                table: "facility_link");

            migrationBuilder.DropColumn(
                name: "id",
                table: "facility_link");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_facility_link",
                table: "facility_link",
                columns: new[] { "facility_id_a", "facility_id_b" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_facility_link",
                table: "facility_link");

            migrationBuilder.AddColumn<string>(
                name: "id",
                table: "facility_link",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_facility_link",
                table: "facility_link",
                column: "id");
        }
    }
}

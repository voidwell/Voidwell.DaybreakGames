using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease31 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "zone_control_ns",
                table: "event_facility_control",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "zone_population_ns",
                table: "event_facility_control",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "last_faction_ns",
                table: "alert",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "start_faction_ns",
                table: "alert",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "zone_control_ns",
                table: "event_facility_control");

            migrationBuilder.DropColumn(
                name: "zone_population_ns",
                table: "event_facility_control");

            migrationBuilder.DropColumn(
                name: "last_faction_ns",
                table: "alert");

            migrationBuilder.DropColumn(
                name: "start_faction_ns",
                table: "alert");
        }
    }
}

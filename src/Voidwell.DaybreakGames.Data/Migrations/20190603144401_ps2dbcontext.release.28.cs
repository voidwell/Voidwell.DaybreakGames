using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ns_avg_play_time",
                table: "daily_population",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ns_count",
                table: "daily_population",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ns_avg_play_time",
                table: "daily_population");

            migrationBuilder.DropColumn(
                name: "ns_count",
                table: "daily_population");
        }
    }
}

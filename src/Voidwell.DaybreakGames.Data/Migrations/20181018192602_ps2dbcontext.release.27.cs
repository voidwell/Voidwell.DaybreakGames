using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "avg_play_time",
                table: "daily_population",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nc_avg_play_time",
                table: "daily_population",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tr_avg_play_time",
                table: "daily_population",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "vs_avg_play_time",
                table: "daily_population",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avg_play_time",
                table: "daily_population");

            migrationBuilder.DropColumn(
                name: "nc_avg_play_time",
                table: "daily_population");

            migrationBuilder.DropColumn(
                name: "tr_avg_play_time",
                table: "daily_population");

            migrationBuilder.DropColumn(
                name: "vs_avg_play_time",
                table: "daily_population");
        }
    }
}

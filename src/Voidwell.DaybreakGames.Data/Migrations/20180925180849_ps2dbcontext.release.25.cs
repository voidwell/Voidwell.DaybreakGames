using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "weapon_aggregate",
                columns: table => new
                {
                    item_id = table.Column<int>(nullable: false),
                    vehicle_id = table.Column<int>(nullable: false),
                    avg_kills = table.Column<float>(nullable: true),
                    std_kills = table.Column<float>(nullable: true),
                    sum_kills = table.Column<long>(nullable: true),
                    avg_deaths = table.Column<float>(nullable: true),
                    std_deaths = table.Column<float>(nullable: true),
                    sum_deaths = table.Column<long>(nullable: true),
                    avg_fire_count = table.Column<float>(nullable: true),
                    std_fire_count = table.Column<float>(nullable: true),
                    sum_fire_count = table.Column<long>(nullable: true),
                    avg_hit_count = table.Column<float>(nullable: true),
                    std_hit_count = table.Column<float>(nullable: true),
                    sum_hit_count = table.Column<long>(nullable: true),
                    avg_headshots = table.Column<float>(nullable: true),
                    std_headshots = table.Column<float>(nullable: true),
                    sum_headshots = table.Column<long>(nullable: true),
                    avg_play_time = table.Column<float>(nullable: true),
                    std_play_time = table.Column<float>(nullable: true),
                    sum_play_time = table.Column<long>(nullable: true),
                    avg_score = table.Column<float>(nullable: true),
                    std_score = table.Column<float>(nullable: true),
                    sum_score = table.Column<long>(nullable: true),
                    avg_vehicle_kills = table.Column<float>(nullable: true),
                    std_vehicle_kills = table.Column<float>(nullable: true),
                    sum_vehicle_kills = table.Column<long>(nullable: true),
                    avg_kdr = table.Column<float>(nullable: true),
                    std_kdr = table.Column<float>(nullable: true),
                    avg_accuracy = table.Column<float>(nullable: true),
                    std_accuracy = table.Column<float>(nullable: true),
                    avg_hsr = table.Column<float>(nullable: true),
                    std_hsr = table.Column<float>(nullable: true),
                    avg_kph = table.Column<float>(nullable: true),
                    std_kph = table.Column<float>(nullable: true),
                    avg_vkph = table.Column<float>(nullable: true),
                    std_vkph = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_weapon_aggregate", x => new { x.item_id, x.vehicle_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weapon_aggregate");
        }
    }
}

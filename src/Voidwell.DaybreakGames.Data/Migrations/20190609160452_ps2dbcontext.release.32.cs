using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease32 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience");

            migrationBuilder.DropIndex(
                name: "i_x_event_gain_experience_world_id_zone_id_timestamp",
                table: "event_gain_experience");

            migrationBuilder.AlterColumn<string>(
                name: "other_id",
                table: "event_gain_experience",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "i_x_event_player_logout_timestamp_world_id",
                table: "event_player_logout",
                columns: new[] { "timestamp", "world_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_player_login_timestamp_world_id",
                table: "event_player_login",
                columns: new[] { "timestamp", "world_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_timestamp_character_id_experience_id",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "character_id", "experience_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_timestamp_world_id_experience_id_zone",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "world_id", "experience_id", "zone_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_event_player_logout_timestamp_world_id",
                table: "event_player_logout");

            migrationBuilder.DropIndex(
                name: "i_x_event_player_login_timestamp_world_id",
                table: "event_player_login");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience");

            migrationBuilder.DropIndex(
                name: "i_x_event_gain_experience_timestamp_character_id_experience_id",
                table: "event_gain_experience");

            migrationBuilder.DropIndex(
                name: "i_x_event_gain_experience_timestamp_world_id_experience_id_zone",
                table: "event_gain_experience");

            migrationBuilder.AlterColumn<string>(
                name: "other_id",
                table: "event_gain_experience",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_event_gain_experience",
                table: "event_gain_experience",
                columns: new[] { "id", "timestamp", "character_id", "experience_id", "other_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_world_id_zone_id_timestamp",
                table: "event_gain_experience",
                columns: new[] { "world_id", "zone_id", "timestamp" });
        }
    }
}

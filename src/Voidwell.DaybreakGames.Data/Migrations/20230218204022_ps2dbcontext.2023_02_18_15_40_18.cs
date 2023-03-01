using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontext2023_02_18_15_40_18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "sanctioned_weapon",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "player_session",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "experience",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "metagame_id",
                table: "event_metagame_event",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.CreateTable(
                name: "achievement",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    objective_group_id = table.Column<int>(type: "integer", nullable: true),
                    reward_id = table.Column<int>(type: "integer", nullable: true),
                    repeatable = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_achievement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_achievement",
                columns: table => new
                {
                    character_id = table.Column<string>(type: "text", nullable: false),
                    achievement_id = table.Column<int>(type: "integer", nullable: false),
                    earned_count = table.Column<int>(type: "integer", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    finish_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_achievement", x => new { x.character_id, x.achievement_id });
                });

            migrationBuilder.CreateTable(
                name: "character_directive_tree",
                columns: table => new
                {
                    character_id = table.Column<string>(type: "text", nullable: false),
                    directive_tree_id = table.Column<int>(type: "integer", nullable: false),
                    current_directive_tier_id = table.Column<int>(type: "integer", nullable: false),
                    current_level = table.Column<int>(type: "integer", nullable: false),
                    completion_time_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_directive_tree", x => new { x.character_id, x.directive_tree_id });
                    table.ForeignKey(
                        name: "f_k_character_directive_tree_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "directive",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    directive_tree_id = table.Column<int>(type: "integer", nullable: false),
                    directive_tier_id = table.Column<int>(type: "integer", nullable: false),
                    objective_set_id = table.Column<int>(type: "integer", nullable: false),
                    qualify_requirement_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_id = table.Column<int>(type: "integer", nullable: true),
                    image_set_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_directive", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "directive_tier",
                columns: table => new
                {
                    directive_tree_id = table.Column<int>(type: "integer", nullable: false),
                    directive_tier_id = table.Column<int>(type: "integer", nullable: false),
                    reward_set_id = table.Column<int>(type: "integer", nullable: true),
                    directive_points = table.Column<int>(type: "integer", nullable: true),
                    completion_count = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    image_id = table.Column<int>(type: "integer", nullable: true),
                    image_set_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_directive_tier", x => new { x.directive_tree_id, x.directive_tier_id });
                });

            migrationBuilder.CreateTable(
                name: "directive_tree",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    directive_tree_category_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_id = table.Column<int>(type: "integer", nullable: true),
                    image_set_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_directive_tree", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "directive_tree_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_directive_tree_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "objective",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    objective_type_id = table.Column<int>(type: "integer", nullable: false),
                    objective_group_id = table.Column<int>(type: "integer", nullable: false),
                    param1 = table.Column<string>(type: "text", nullable: true),
                    param2 = table.Column<string>(type: "text", nullable: true),
                    param3 = table.Column<string>(type: "text", nullable: true),
                    param4 = table.Column<string>(type: "text", nullable: true),
                    param5 = table.Column<string>(type: "text", nullable: true),
                    param6 = table.Column<string>(type: "text", nullable: true),
                    param7 = table.Column<string>(type: "text", nullable: true),
                    param8 = table.Column<string>(type: "text", nullable: true),
                    param9 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_objective", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "objective_set_to_objective",
                columns: table => new
                {
                    objective_set_id = table.Column<int>(type: "integer", nullable: false),
                    objective_group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_objective_set_to_objective", x => x.objective_set_id);
                });

            migrationBuilder.CreateTable(
                name: "reward",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    reward_type_id = table.Column<int>(type: "integer", nullable: false),
                    count_min = table.Column<int>(type: "integer", nullable: false),
                    count_max = table.Column<int>(type: "integer", nullable: false),
                    param1 = table.Column<int>(type: "integer", nullable: true),
                    param2 = table.Column<int>(type: "integer", nullable: true),
                    param3 = table.Column<int>(type: "integer", nullable: true),
                    param4 = table.Column<int>(type: "integer", nullable: true),
                    param5 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_reward", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reward_group_to_reward",
                columns: table => new
                {
                    reward_group_id = table.Column<int>(type: "integer", nullable: false),
                    reward_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_reward_group_to_reward", x => new { x.reward_group_id, x.reward_id });
                });

            migrationBuilder.CreateTable(
                name: "reward_set_to_reward_group",
                columns: table => new
                {
                    reward_set_id = table.Column<int>(type: "integer", nullable: false),
                    reward_group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_reward_set_to_reward_group", x => new { x.reward_set_id, x.reward_group_id });
                });

            migrationBuilder.CreateTable(
                name: "character_directive",
                columns: table => new
                {
                    character_id = table.Column<string>(type: "text", nullable: false),
                    directive_id = table.Column<int>(type: "integer", nullable: false),
                    directive_tree_id = table.Column<int>(type: "integer", nullable: false),
                    completion_time_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_directive", x => new { x.character_id, x.directive_id });
                    table.ForeignKey(
                        name: "f_k_character_directive_character_directive_tree_character_id_dir",
                        columns: x => new { x.character_id, x.directive_tree_id },
                        principalTable: "character_directive_tree",
                        principalColumns: new[] { "character_id", "directive_tree_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_directive_tier",
                columns: table => new
                {
                    character_id = table.Column<string>(type: "text", nullable: false),
                    directive_tree_id = table.Column<int>(type: "integer", nullable: false),
                    directive_tier_id = table.Column<int>(type: "integer", nullable: false),
                    completion_time_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_directive_tier", x => new { x.character_id, x.directive_tree_id, x.directive_tier_id });
                    table.ForeignKey(
                        name: "f_k_character_directive_tier_character_directive_tree_character",
                        columns: x => new { x.character_id, x.directive_tree_id },
                        principalTable: "character_directive_tree",
                        principalColumns: new[] { "character_id", "directive_tree_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_directive_objective",
                columns: table => new
                {
                    character_id = table.Column<string>(type: "text", nullable: false),
                    directive_id = table.Column<int>(type: "integer", nullable: false),
                    objective_id = table.Column<int>(type: "integer", nullable: false),
                    objective_group_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    state_data = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_directive_objective", x => new { x.character_id, x.directive_id, x.objective_id });
                    table.ForeignKey(
                        name: "f_k_character_directive_objective_character_directive_character",
                        columns: x => new { x.character_id, x.directive_id },
                        principalTable: "character_directive",
                        principalColumns: new[] { "character_id", "directive_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_character_directive_character_id_directive_tree_id",
                table: "character_directive",
                columns: new[] { "character_id", "directive_tree_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "achievement");

            migrationBuilder.DropTable(
                name: "character_achievement");

            migrationBuilder.DropTable(
                name: "character_directive_objective");

            migrationBuilder.DropTable(
                name: "character_directive_tier");

            migrationBuilder.DropTable(
                name: "directive");

            migrationBuilder.DropTable(
                name: "directive_tier");

            migrationBuilder.DropTable(
                name: "directive_tree");

            migrationBuilder.DropTable(
                name: "directive_tree_category");

            migrationBuilder.DropTable(
                name: "objective");

            migrationBuilder.DropTable(
                name: "objective_set_to_objective");

            migrationBuilder.DropTable(
                name: "reward");

            migrationBuilder.DropTable(
                name: "reward_group_to_reward");

            migrationBuilder.DropTable(
                name: "reward_set_to_reward_group");

            migrationBuilder.DropTable(
                name: "character_directive");

            migrationBuilder.DropTable(
                name: "character_directive_tree");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "sanctioned_weapon",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "player_session",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "experience",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "metagame_id",
                table: "event_metagame_event",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}

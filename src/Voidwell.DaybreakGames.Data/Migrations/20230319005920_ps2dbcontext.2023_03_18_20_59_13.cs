using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontext2023_03_18_20_59_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "image_set",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    image_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_image_set", x => new { x.id, x.type_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image_set");
        }
    }
}

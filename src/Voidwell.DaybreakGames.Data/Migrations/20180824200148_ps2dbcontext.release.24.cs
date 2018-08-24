using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class ps2dbcontextrelease24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_rating",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    rating = table.Column<double>(nullable: false),
                    deviation = table.Column<double>(nullable: false),
                    volatility = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_rating", x => x.character_id);
                    table.ForeignKey(
                        name: "f_k_character_rating_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_character_rating_rating",
                table: "character_rating",
                column: "rating");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_rating");
        }
    }
}

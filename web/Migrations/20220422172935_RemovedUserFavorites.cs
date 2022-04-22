using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedUserFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavorites",
                schema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavorites",
                schema: "app",
                columns: table => new
                {
                    UserFavoritesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserFavo__1D23EA13087A927D", x => x.UserFavoritesId);
                    table.ForeignKey(
                        name: "FK_UserFavorites_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_UserFavorites_UserFavoriteFolders",
                        column: x => x.FolderId,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_FolderId",
                schema: "app",
                table: "UserFavorites",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId",
                schema: "app",
                table: "UserFavorites",
                column: "UserId");
        }
    }
}

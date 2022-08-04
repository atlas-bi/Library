using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredUserPreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from app.userpreferences
                where userid is null
            ");
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_User",
                schema: "app",
                table: "UserPreferences");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_User",
                schema: "app",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_User",
                schema: "app",
                table: "UserPreferences");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "UserPreferences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_User",
                schema: "app",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID");
        }
    }
}

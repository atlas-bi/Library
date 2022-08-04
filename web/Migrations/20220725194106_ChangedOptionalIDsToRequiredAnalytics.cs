using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredAnalytics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from app.AnalyticsTrace where userid is null");
            migrationBuilder.Sql(@"delete from app.AnalyticsError where userid is null");
            migrationBuilder.Sql(@"delete from app.Analytics where userid is null");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_User",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Error_User",
                schema: "app",
                table: "AnalyticsError");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Trace_User",
                schema: "app",
                table: "AnalyticsTrace");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "AnalyticsTrace",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "AnalyticsError",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "Analytics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_User",
                schema: "app",
                table: "Analytics",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Error_User",
                schema: "app",
                table: "AnalyticsError",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Trace_User",
                schema: "app",
                table: "AnalyticsTrace",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_User",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Error_User",
                schema: "app",
                table: "AnalyticsError");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Trace_User",
                schema: "app",
                table: "AnalyticsTrace");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "AnalyticsTrace",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "AnalyticsError",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "Analytics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_User",
                schema: "app",
                table: "Analytics",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Error_User",
                schema: "app",
                table: "AnalyticsError",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Trace_User",
                schema: "app",
                table: "AnalyticsTrace",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID");
        }
    }
}

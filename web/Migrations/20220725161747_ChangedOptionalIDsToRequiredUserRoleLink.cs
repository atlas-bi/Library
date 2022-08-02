using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredUserRoleLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from app.userrolelinks
            where userid is null or userrolesid is null"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleLinks_User",
                schema: "app",
                table: "UserRoleLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleLinks_UserRoles",
                schema: "app",
                table: "UserRoleLinks");

            migrationBuilder.AlterColumn<int>(
                name: "UserRolesId",
                schema: "app",
                table: "UserRoleLinks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "UserRoleLinks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleLinks_User",
                schema: "app",
                table: "UserRoleLinks",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleLinks_UserRoles",
                schema: "app",
                table: "UserRoleLinks",
                column: "UserRolesId",
                principalSchema: "app",
                principalTable: "UserRoles",
                principalColumn: "UserRolesId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleLinks_User",
                schema: "app",
                table: "UserRoleLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleLinks_UserRoles",
                schema: "app",
                table: "UserRoleLinks");

            migrationBuilder.AlterColumn<int>(
                name: "UserRolesId",
                schema: "app",
                table: "UserRoleLinks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "app",
                table: "UserRoleLinks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleLinks_User",
                schema: "app",
                table: "UserRoleLinks",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleLinks_UserRoles",
                schema: "app",
                table: "UserRoleLinks",
                column: "UserRolesId",
                principalSchema: "app",
                principalTable: "UserRoles",
                principalColumn: "UserRolesId");
        }
    }
}

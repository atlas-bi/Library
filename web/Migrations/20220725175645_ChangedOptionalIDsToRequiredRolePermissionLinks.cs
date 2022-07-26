using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredRolePermissionLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from app.RolePermissionLinks
            where roleid is null or RolePermissionLinksId is null
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionLinks_RolePermissions",
                schema: "app",
                table: "RolePermissionLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionLinks_UserRoles",
                schema: "app",
                table: "RolePermissionLinks");

            migrationBuilder.AlterColumn<int>(
                name: "RolePermissionsId",
                schema: "app",
                table: "RolePermissionLinks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                schema: "app",
                table: "RolePermissionLinks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionLinks_RolePermissions",
                schema: "app",
                table: "RolePermissionLinks",
                column: "RolePermissionsId",
                principalSchema: "app",
                principalTable: "RolePermissions",
                principalColumn: "RolePermissionsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionLinks_UserRoles",
                schema: "app",
                table: "RolePermissionLinks",
                column: "RoleId",
                principalSchema: "app",
                principalTable: "UserRoles",
                principalColumn: "UserRolesId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionLinks_RolePermissions",
                schema: "app",
                table: "RolePermissionLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionLinks_UserRoles",
                schema: "app",
                table: "RolePermissionLinks");

            migrationBuilder.AlterColumn<int>(
                name: "RolePermissionsId",
                schema: "app",
                table: "RolePermissionLinks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                schema: "app",
                table: "RolePermissionLinks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionLinks_RolePermissions",
                schema: "app",
                table: "RolePermissionLinks",
                column: "RolePermissionsId",
                principalSchema: "app",
                principalTable: "RolePermissions",
                principalColumn: "RolePermissionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionLinks_UserRoles",
                schema: "app",
                table: "RolePermissionLinks",
                column: "RoleId",
                principalSchema: "app",
                principalTable: "UserRoles",
                principalColumn: "UserRolesId");
        }
    }
}

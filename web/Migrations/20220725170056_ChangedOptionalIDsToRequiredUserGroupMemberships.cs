using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredUserGroupMemberships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from dbo.usergroupsmembership
            where userid is null or groupid is null
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupsMembership_User",
                table: "UserGroupsMembership");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupsMembership_UserGroups",
                table: "UserGroupsMembership");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserGroupsMembership",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "UserGroupsMembership",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupsMembership_User",
                table: "UserGroupsMembership",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupsMembership_UserGroups",
                table: "UserGroupsMembership",
                column: "GroupId",
                principalTable: "UserGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupsMembership_User",
                table: "UserGroupsMembership");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupsMembership_UserGroups",
                table: "UserGroupsMembership");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserGroupsMembership",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "UserGroupsMembership",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupsMembership_User",
                table: "UserGroupsMembership",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupsMembership_UserGroups",
                table: "UserGroupsMembership",
                column: "GroupId",
                principalTable: "UserGroups",
                principalColumn: "GroupId");
        }
    }
}

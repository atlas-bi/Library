using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddGroupRoleLinksModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupRoleLinks",
                schema: "app",
                columns: table => new
                {
                    GroupRoleLinksId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserRolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GroupRole__LinkId", x => x.GroupRoleLinksId);
                    table.ForeignKey(
                        name: "FK_GroupRoleLinks_Group",
                        column: x => x.GroupId,
                        principalSchema: "dbo",
                        principalTable: "UserGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRoleLinks_UserRoles",
                        column: x => x.UserRolesId,
                        principalSchema: "app",
                        principalTable: "UserRoles",
                        principalColumn: "UserRolesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "groupid+roleid",
                schema: "app",
                table: "GroupRoleLinks",
                columns: new[] { "GroupId", "UserRolesId" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoleLinks_UserRolesId",
                schema: "app",
                table: "GroupRoleLinks",
                column: "UserRolesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoleLinks",
                schema: "app");
        }
    }
}

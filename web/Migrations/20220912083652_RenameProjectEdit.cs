using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RenameProjectEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            update app.RolePermissions set name = 'Edit Collection' where name = 'Edit Project'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            update app.RolePermissions set name = 'Edit Project' where name = 'Edit Collection'");
        }
    }
}
